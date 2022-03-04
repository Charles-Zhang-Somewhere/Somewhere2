using Terminal.Gui;
using NStack;
using YamlDotNet.Core.Events;

namespace Somewhere2.TUIApplication.Applet
{
    public class FileBrowser
    {
        public string InitialPath { get; }
        public string FinalPath { get; set; }
        public FileBrowser(string initialPath)
        {
            InitialPath = initialPath;
        }

        public string Start()
        {
            Application.Init();
            Toplevel top = Application.Top;
            
            Window win = new Window($"File Browser - {InitialPath}")
            {
                X = 0,
                Y = 1, // Leave one row for the toplevel menu
                
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            top.Add(win);
            
            MenuBar menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_New", "Creates new file", null),
                    new MenuItem ("_Close", "",null),
                    new MenuItem ("_Quit", "", () => { if (Quit ()) top.Running = false; })
                }),
                new MenuBarItem ("_Edit", new MenuItem [] {
                    new MenuItem ("_Copy", "", null),
                    new MenuItem ("C_ut", "", null),
                    new MenuItem ("_Paste", "", null)
                })
            });
            top.Add(menu);

            Label login = new Label("Login: ") { X = 3, Y = 2 };
            Label password = new Label("Password: ")
            {
                X = Pos.Left(login),
                Y = Pos.Top(login) + 1
            };
            TextField loginText = new TextField("")
            {
                X = Pos.Right(password),
                Y = Pos.Top(login),
                Width = 40
            };
            TextField passText = new TextField("")
            {
                Secret = true,
                X = Pos.Left(loginText),
                Y = Pos.Top(password),
                Width = Dim.Width(loginText)
            };
            
            win.Add(
                // The ones with layout system automatically computed
                login, password, loginText, passText,

                // The ones laid out like an australopithecus, with Absolute positions:
                new CheckBox(3, 6, "Remember me"),
                new RadioGroup(3, 8, new ustring[] { "_Personal", "_Company" }, 0),
                new Button(3, 14, "Ok"),
                new Button(10, 14, "Cancel"),
                new Label(3, 18, "Press F9 or ESC plus 9 to activate the menubar")
            );

            Application.Run();

            return FinalPath;
        }
        
        static bool Quit()
        {
            int n = MessageBox.Query(50, 7, "Quit Demo", "Are you sure you want to quit this demo?", "Yes", "No");
            return n == 0;
        }
    }
}