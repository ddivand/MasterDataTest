using System;
using System.Threading;
using System.Windows.Automation;
using System.Diagnostics;


namespace MasterDataTest
{
    class Program
    {
        static String user = "testuser";
        static String pass = "testpassword";

        static void Main(string[] args)
        {
            bool isRunning = false;

            //changing username and password
            if (args.Length > 0 && args.Length < 3)
            {                
                user = args[0];
                pass = args[1];
            }
            
            // start up AuthTest if needed
            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                if (!String.IsNullOrEmpty(p.MainWindowTitle) && p.MainWindowTitle == "MainWindow")
                    isRunning = true;                
            }

            const String fileName = "AuthTest.exe";
            try
            {
                if (isRunning == false)
                {
                    Process p = Process.Start(fileName);
                    Thread.Sleep(500);

                    if( p == null)
                    {
                        Console.WriteLine("Error: couldn't start " + fileName);
                        Environment.Exit(1);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message +": " + fileName);
                Environment.Exit(1);
            }


            if (Automate() != 0)
                Environment.Exit(1);
        }

        static int Automate()
        {
            AutomationElement root;            

            Console.WriteLine("Searching for root element");
            root = AutomationElement.RootElement;

            if (root == null)
            {
                Console.WriteLine("Error: tree root not found");
                return 1;
            }

            Console.WriteLine("root found");

            if (AutomateMainWindow(root) != 0)
            {
                Console.WriteLine("Error: MainWindow not found");
                return 1;
            }            

            return 0;
        }

        static int AutomateMainWindow(AutomationElement root)
        {            
            AutomationElement mainWindow;
                        
            Console.WriteLine("Searching for MainWindow");
            
            //searching for MainWindow and LogIn button                        
            mainWindow = root.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "MainWindow"));

            if (mainWindow == null)                          
                return 1;            

            AutomationElement buttonLogIn = mainWindow.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, ""));

            if (buttonLogIn == null)
                return 1;

            InvokePattern ivkp = buttonLogIn.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            ivkp.Invoke();


            Thread.Sleep(500);


            if (AutomateMyserverConnect(root) != 0)
            {
                Console.WriteLine("Error: Connect to myserver window not found");
                return 1;
            }

            Thread.Sleep(1000);

            //searching for last window and OK button            
            AutomationElement buttonOk = mainWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "2"));

            if (buttonOk == null)
                return 1;

            InvokePattern ivkOk = buttonOk.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            ivkOk.Invoke();

            return 0;

        }

        static int AutomateMyserverConnect(AutomationElement root)
        {
            Console.WriteLine("Searching for connection window");
            AutomationElement myserverConnect = root.FindFirst(TreeScope.Children, 
                new PropertyCondition(AutomationElement.NameProperty, "Подключение к myserver"));

            if (myserverConnect == null)
                return 1;

            //searching for neccesary elements            
            AutomationElement textLogin = myserverConnect.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "1003"));
            AutomationElement textPassword = myserverConnect.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "1005"));
            AutomationElement buttonOK = myserverConnect.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "1"));

            if (textLogin == null || textPassword == null || buttonOK == null)
                return 1;

            // setting user, password and pressing OK button
            ValuePattern login = textLogin.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            ValuePattern password = textPassword.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;

            login.SetValue(user);
            password.SetValue(pass);

            InvokePattern ivkp = buttonOK.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            ivkp.Invoke();

            return 0;
        }
    }
}
