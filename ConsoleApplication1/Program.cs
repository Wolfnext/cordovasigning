using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CordovaSigning
{
    class Program
    {
        const String noJava= "Nie masz zainstalowanej Javy.";
        const String javaVersionText = "Twoja wersja Java to: ";
        const String startSinging = "Zaczynam podpisywanie";
        const String fileNametext = "Nazwa Twojego pliku ? ";
        const String keystoreNametext = "Nazwa Twojego pliku podpisu ?";
        const String aliasNametext = "Nazwa Twojego aliasu ? ";
        const String versionSDKtext = "Twoja wersja SDK ? ";
        const String outputFileNametext = "Nazwa pliku wyjściowego ? ";
        const String correctDatatext = "Czy dane są poprawne? (Wciśnij  T jesli TAK lub N jeśli nie)";
        const String checkExistKeystoreFiletext = "Sprawdzam czy istnieje plik keystore w folderze Java. Jeśli nic sie nie dzieje, skopiuj  swój do otwartego folderu i go zamknij.";
        const String singingdone = "Podpisywanie przebiegło pomyślnie.";
        static String folderJava = null;
        static String folderAndroidSDK = @"C:\Program Files (x86)\Android\android-sdk\build-tools";
        static String fileName = "app-release-unsigned";
        static String keystoreName = null;
        static String outputFileName = null;
        static String aliasName =null;
        static bool keystoreExist = false;
        static string versionSDK = null;

        static void Main(string[] args)
        {
            Console.WriteLine(GetJavaVersionInformation());
            GetInfoAboutSinging();
            GetJavaInstallationPath();
            checkExistKeystoreFile();
            moveAppFileIfNotExist();
            jarSingerSinging();
            moveSingnerApp();
            androidZiper();
            checkfile();
            Console.ReadLine();
        }
        static void moveSingnerApp()
        {
            File.Move(@"" + folderJava + "\\bin\\" + fileName + ".apk", @"" + folderAndroidSDK + "\\"+versionSDK+"\\" + fileName + ".apk");
        }

        static void checkfile()
        {
            File.Delete(@"C:\Program Files (x86)\Android\android-sdk\build-tools\" + versionSDK + "\\"+fileName+".apk");
            if (File.Exists(outputFileName + ".apk"))
            {
                Console.Clear();
                Console.WriteLine(singingdone);
            }
           
        }
        static void androidZiper()
        {
            try
            {
                  ProcessStartInfo psi = new ProcessStartInfo();
                  psi.FileName = @"C:\Program Files (x86)\Android\android-sdk\build-tools\"+versionSDK+"\\zipalign";
                  psi.Arguments = @"-v 4 "+fileName+".apk "+outputFileName+".apk";
                  psi.CreateNoWindow = true;
                  psi.UseShellExecute = false;
                  Process p = Process.Start(psi);
                  p.WaitForExit();
            }
            catch (Exception e) { Console.WriteLine("Error" + e); }
            }

        static void moveAppFileIfNotExist()
        {
            if (File.Exists(@"" + folderJava + "\\bin\\" + fileName + ".apk"))
            {
                Console.WriteLine("istnieje");
            }
            else
            {
                    File.Copy(@"" + fileName+".apk", @"" + folderJava + "\\bin\\"+ fileName + ".apk");  
            }
        }
        static void checkExistKeystoreFile()
        {
            bool openWindow = false;
            Console.WriteLine(checkExistKeystoreFiletext);
            do {
                if (File.Exists(@"" + folderJava +"\\bin\\"+ keystoreName + ".keystore"))
                {
                    keystoreExist = true;
                }
                else{if(!openWindow) Process.Start(@"" + folderJava + "\\bin\\");openWindow = true; }
            } while (keystoreExist==false);
        }
        static string GetJavaVersionInformation()
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey subKey = rk.OpenSubKey("SOFTWARE\\JavaSoft\\Java Development Kit");
                string currentVerion = subKey.GetValue("CurrentVersion").ToString();
                return javaVersionText + currentVerion;
            }
            catch (Exception e) {return noJava;}

        }

       static void GetJavaInstallationPath()
        {
            try
            {
                string environmentPath = Environment.GetEnvironmentVariable("JAVA_HOME");
                if (!string.IsNullOrEmpty(environmentPath))
                {
                    folderJava = environmentPath;
                }

                string javaKey = "SOFTWARE\\JavaSoft\\Java Development Kit\\";
                using (Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(javaKey))
                {
                    string currentVersion = rk.GetValue("CurrentVersion").ToString();
                    using (Microsoft.Win32.RegistryKey key = rk.OpenSubKey(currentVersion))
                    {
                        folderJava = key.GetValue("JavaHome").ToString();
                    }
                }
            }catch(Exception e) { };
        }
        static void GetInfoAboutSinging()
        {
            ConsoleKey response;
            do
            { 
                Console.WriteLine(versionSDKtext);
                versionSDK = Console.ReadLine();
                Console.WriteLine(keystoreNametext);
                keystoreName = Console.ReadLine();
                Console.WriteLine(aliasNametext);
                aliasName = Console.ReadLine();
                Console.WriteLine(outputFileNametext);
                outputFileName = Console.ReadLine();
                Console.Clear();
                Console.WriteLine(versionSDK);
                Console.WriteLine(keystoreName);
                Console.WriteLine(aliasName);
                Console.WriteLine(outputFileName);
                do
                {
                    Console.WriteLine(correctDatatext);
                    response = Console.ReadKey(false).Key;
                } while (response != ConsoleKey.T && response != ConsoleKey.N);
                Console.Clear();
            } while (response != ConsoleKey.T);
            Console.Clear();
        }

        static void jarSingerSinging()
        {
            if(keystoreName!=null && fileName != null && aliasName!= null)
            {
                try {
                    Console.WriteLine(startSinging);

                    Process cmd = System.Diagnostics.Process.Start(@"" + folderJava + "\\bin\\jarsigner.exe",
                    @"-verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore "+ keystoreName + ".keystore " + fileName + ".apk " + aliasName);
                    cmd.WaitForExit();
                } catch (Exception e) { Console.WriteLine("Error" +e);  }
            }
        }

    }
}
