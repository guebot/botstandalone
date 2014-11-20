using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public static class Log 
{
    public static void WriteToLog(TextBox textBox, string message)
    {
        textBox.AppendText(message);
        textBox.AppendText(Environment.NewLine);
        WriteToLog(message);
    }

    public static void WriteToLog(TextBox textBox, string message, params object[] args)
    {
        textBox.AppendText(string.Format(message, args));
        textBox.AppendText(Environment.NewLine);
        WriteToLog(message,args);
    }

    public static void WriteToLog(string message, params object[] args)
    {
        Console.WriteLine(message, args);
        //Console.WriteLine("Texto Prueba");
    }

    public static void WriteToLog(string message)
    {
        Console.WriteLine(message);
        //Console.WriteLine("Texto Prueba");
    }

}
