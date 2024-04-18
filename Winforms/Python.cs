using System.Collections.Generic;
using System.Drawing;
using Python.Runtime;

public class PythonNet
{
    public PythonNet()
    {
        Runtime.PythonDLL = "python311.dll";
        PythonEngine.Initialize();
        dynamic tf = Py.Import("tensorflow");
        dynamic np = Py.Import("numpy");
        dynamic cv = Py.Import("opencv-python");

    }

    public List<Bitmap> GetAllWords()
    {

        return null;
    }

}