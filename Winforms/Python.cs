using Python.Runtime;

public class PythonNet
{
    public PythonNet()
    {
        Runtime.PythonDLL = "python311.dll";
        PythonEngine.Initialize();
    }
}