
using Shimakaze.UI.Native;
using Shimakaze.UI.Native.Gtk4;
using Shimakaze.UI.Native.Win32;

Dispatcher dispatcher = new Gtk4Dispatcher();
Application app = new Gtk4Application(dispatcher);
app.Initialize += (_, _) =>
{
    Window window = new Gtk4Window();
    window.Show();
};
app.Run();

//Dispatcher dispatcher = new Win32Dispatcher();
//Application app = new Win32Application(dispatcher);
//app.Initialize += (_, _) =>
//{
//    Window window = new Win32Window();
//    window.Show();
//};
//app.Run();