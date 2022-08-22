using System.Diagnostics;

Console.WriteLine($"Preparing IL modification of Lod.RecordCollections to '{args.FirstOrDefault()}'");

string dllPath = !string.IsNullOrWhiteSpace(args.FirstOrDefault()) ? args.First()
    : throw new ArgumentException("Lod.RecordCollections DLL not found in args.");
if (!File.Exists(dllPath)) throw new FileNotFoundException("Lod.RecordCollections.dll");
string directory = Directory.GetCurrentDirectory();
Console.WriteLine($"Searching for ilasm/ildasm in directory '{directory}'.");
string ilasmPath = Directory.EnumerateFiles(directory, "ilasm.exe", SearchOption.AllDirectories).FirstOrDefault()
    ?? throw new FileNotFoundException("ilasm.exe");
string ildasmPath = Directory.EnumerateFiles(directory, "ildasm.exe", SearchOption.AllDirectories).FirstOrDefault()
    ?? throw new FileNotFoundException("ildasm.exe");


// decompile
Console.WriteLine("Decompiling IL.");

string fileName = "Lod.RecordCollections.il";
string ilPath = Path.Combine(directory, fileName);

Process dasmProcess = Process.Start(ildasmPath, $"\"{dllPath}\" /out:\"{ilPath}\"");
#if DEBUG
dasmProcess.OutputDataReceived += (s, e) => Console.WriteLine(e?.Data);
#endif
await dasmProcess.WaitForExitAsync();
if (dasmProcess.ExitCode != 0) throw new InvalidOperationException("IL decompilation failed.");


// modify
Console.WriteLine("Modifying IL");

string fileContent = await File.ReadAllTextAsync(ilPath);
Type[] collectionNames = new[]
{
    typeof(RecordList<byte>).GetGenericTypeDefinition(),
    typeof(RecordSet<byte>).GetGenericTypeDefinition(),
    typeof(RecordDictionary<byte, byte>).GetGenericTypeDefinition(),
};
string cloneTemplate = @"
    .method public hidebysig newslot virtual 
        instance class $!TYPE!$<!T> '<Clone>$' () cil managed 
    {
        .maxstack 24
        .locals init (
            [0] class $!TYPE!$<!T>
        )

        IL_0000: nop
        IL_0001: ldarg.0
        IL_0002: newobj instance void class $!TYPE!$<!T>::.ctor(class $!TYPE!$<!0>)
        IL_0007: stloc.0
        IL_0008: br.s IL_000a

        IL_000a: ldloc.0
        IL_000b: ret
    } // end of method $!TYPE!$::'<Clone>$'
";
foreach (Type collection in collectionNames)
{
    string typeName = collection.FullName!;
    Type[] genericArgs = collection.GetGenericArguments();
    string generics = $"<{string.Join(", ", genericArgs.Select(a => $"!{a.Name}"))}>";
    string genericIndexes = $"<{string.Join(", ", genericArgs.Select((_, i) => $"!{i}"))}>";
    string template = cloneTemplate.Replace("$!TYPE!$", typeName)
        .Replace("<!T>", generics)
        .Replace("<!0>", genericIndexes);

    fileContent = fileContent.Replace($"}} // end of class {typeName}", template + "}");
}
await File.WriteAllTextAsync(ilPath, fileContent);


// recompile
Console.WriteLine("Compile IL");

Process asmProcess = Process.Start(ilasmPath, $"/dll \"{ilPath}\" /output:\"{dllPath}\"");
#if DEBUG
asmProcess.OutputDataReceived += (s, e) => Console.WriteLine(e?.Data);
#endif
await asmProcess.WaitForExitAsync();
if (asmProcess.ExitCode != 0) throw new InvalidOperationException("IL compilation failed.");


#if DEBUG
Console.ReadKey();
#endif