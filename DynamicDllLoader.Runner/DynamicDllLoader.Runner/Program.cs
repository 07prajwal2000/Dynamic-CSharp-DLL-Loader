using System;
using System.Reflection;

var fileName = @"{ABSOLUTE PATH}\DynamicDllLoader.AppOne.dll";

var fileNameWithDeps = @"{ABSOLUTE PATH}\DynamicDllLoader.AppTwoWithDeps.dll";

if (!File.Exists(fileName))
{
    Console.Error.WriteLine("File path doesn't exist");
    Environment.Exit(1);
    return;
}

AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

var loadedAsm = AppDomain.CurrentDomain.Load(File.ReadAllBytes(fileNameWithDeps));

var filteredTypes = loadedAsm.ExportedTypes
    .Where(x => !x.IsInterface || !x.IsAbstract)
    .Where(x => x.IsAssignableTo(x.GetInterface("IInitializer")));

if (filteredTypes.Count() != 1)
{
    Console.WriteLine("Error: found multiple initializer classes");
    return;
}

try
{
    var initializerType = filteredTypes.First();
    Console.WriteLine($"Found {initializerType.Name} class.");
    var initializer = Activator.CreateInstance(initializerType);

    var startFunc = initializerType.GetMethod("Start");
    startFunc?.Invoke(initializer, null);

    var stopFunc = initializerType.GetMethod("Stop");
    stopFunc?.Invoke(initializer, null);
}
catch (Exception e)
{
    Console.WriteLine("Failed to execute the provided DLL.\nMessage: " + e);
}

Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs resolveEventArgs)
{
    var path = Path.Combine(Directory.GetParent(fileNameWithDeps).FullName, new AssemblyName(resolveEventArgs.Name).Name + ".dll");
    if (!File.Exists(path)) return null;
    return Assembly.LoadFile(path);
}