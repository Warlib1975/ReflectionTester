using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReflectionTester
{
    public class MyClass : Second
    {
        private void First_OnTestDelegateReflection(string a, int b)
        {
            Console.WriteLine("\r\nEvent OnTestDelegateReflection has fired. Result a: " + a + ", result b: " + b.ToString());
        }
    }

    class Program
    {
        public static BindingFlags Flags = BindingFlags.Instance
                                           | BindingFlags.GetProperty
                                           | BindingFlags.SetProperty
                                           | BindingFlags.GetField
                                           | BindingFlags.SetField
                                           | BindingFlags.NonPublic;
        static StringBuilder sb = new StringBuilder();

        static void Main(string[] args)
        {
            First first = new First();
            first.OnTestDelegate += First_OnTestDelegate;

            TestReflection(first, BindingFlags.Default, false);

            Second second = new Second();

            second.OnTestDelegate += First_OnTestDelegate;

            TestReflection(second, BindingFlags.Default, false);

            TestReflection(second, BindingFlags.Default, true);

            TestReflection(second, BindingFlags.DeclaredOnly, true);

            Console.ReadLine();

        }

        public static void TestReflection(object obj, BindingFlags flags, bool BaseType = false)
        {
            sb.AppendLine();
            string declared = string.Empty;
            if (flags == BindingFlags.DeclaredOnly)
                declared = "Turned on BindingFlags.DeclaredOnly.";
            else
                declared = "Turned off BindingFlags.DeclaredOnly.";

            declared += BaseType ? " BasedType is TRUE." : " BasedType is FALSE.";

            sb.AppendLine("Use [" + obj.GetType() + "]. " + declared);
            sb.AppendLine("------------------------------------------------------");

            GetListOfFields(obj, flags, BaseType);
            Console.WriteLine(sb.ToString());
            sb.Clear();

            GetListOfMethods(obj, flags, BaseType);
            Console.WriteLine(sb.ToString());
            sb.Clear();

            ExecMethod(obj, "Method1", new object[] { "1016", 1675 }, flags, BaseType);
            Console.WriteLine(sb.ToString());
            sb.Clear();

            Type type = typeof(Program);
            MethodInfo method = type.GetMethod("First_OnTestDelegateReflection", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (method != null)
            {
                SubscribeEvent(obj, obj.GetType(), typeof(TestDelegate), "OnTestDelegate", method, true);
            }
            else
            {
                MessageBox.Show("Couldn't find method \"btnClose_OnClick\".");
            }
        }

        private static void First_OnTestDelegate(string a, int b)
        {
            Console.WriteLine("\r\nEvent OnTestDelegate has fired. Result a: " + a + ", result b: " + b.ToString());
        }

        private static void First_OnTestDelegateReflection(string a, int b)
        {
            Console.WriteLine("\r\nEvent OnTestDelegateReflection has fired. Result a: " + a + ", result b: " + b.ToString());
        }

        private void ChangePrivateFieldClassFirst()
        {

        }



        /// <summary>
        /// A static method to get the FieldInfo of a private field of any object.
        /// </summary>
        /// <param name="type">The Type that has the private field</param>
        /// <param name="fieldName">The name of the private field</param>
        /// <returns>FieldInfo object. It has the field name and a useful GetValue() method.</returns>
        public static FieldInfo GetPrivateFieldInfo(Type type, string fieldName, BindingFlags flags = BindingFlags.Default)
        {
            if (flags == BindingFlags.Default)
                flags = Flags;

            FieldInfo[] fields = type.GetFields(flags);
            if (fields != null)
            {
                foreach (FieldInfo fi in fields) //fields.FirstOrDefault(feildInfo => feildInfo.Name == fieldName);
                {
                    if (fi.Name.ToLower() == fieldName.ToLower())
                        return fi;
                }
            }
            else
            {
                sb.AppendLine("There is no any field found.");
            }
            return null;
        }

        /// <summary>
        /// A static method to get the FieldInfo of a private field of any object.
        /// </summary>
        /// <param name="type">The Type that has the private field</param>
        /// <param name="fieldName">The name of the private field</param>
        /// <param name="o">The instance from which to read the private value.</param>
        /// <returns>The value of the property boxed as an object.</returns>
        public static object GetPrivateFieldValue(Type type, string fieldName, object o, BindingFlags flags = BindingFlags.Default)
        {
            FieldInfo fi = GetPrivateFieldInfo(type, fieldName, flags);
            if (fi != null)
            {
                return fi.GetValue(o);
            }
            else
            {
                sb.AppendLine("Field with the name [" + fieldName + "] is not found.");
            }
            return null;
        }

        public static bool SetPrivateFieldValue(object obj, string fieldName, object value, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            Type type = obj.GetType();

            if (BaseType)
            {
                type = type.BaseType; 
            }

            FieldInfo info = GetPrivateFieldInfo(type, fieldName, flags);
            if (info != null)
            {
                info.SetValue(obj, value);
                return true;
            }
            else
            {
                sb.AppendLine("Field [" + fieldName + "] is not found to SetPrivateFieldValue.");
                return false;
            }
        }

        public static string GetFieldScope(FieldInfo fi)
        {
            if (fi.Attributes == FieldAttributes.Private)
                return "private";
            if (fi.Attributes == FieldAttributes.Family)
                return "protected";

            return fi.Attributes.ToString(); 
        }

        public static string GetMethodScope(MethodInfo mi)
        {
            if (mi.IsPrivate)
                return "private";
            if (mi.IsPublic)
                return "public";
            if (mi.Attributes == MethodAttributes.Family)
                return "protected";
            return mi.Attributes.ToString();
        }

        public static MethodInfo[] GetListOfMethods(object obj, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            Type type = obj.GetType();
            flags = Flags | flags;

            if (BaseType)
            {
                type = type.BaseType; // BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);//  BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            }

            // get all public static methods of MyClass type
            MethodInfo[] methodInfos = type.GetMethods(flags);
            sb.AppendLine("List of private Methods of the class [" + type.Name + "]:");
            foreach (MethodInfo mi in methodInfos)
            {
                string args = "(";
                foreach (ParameterInfo arg in mi.GetParameters())
                {
                    args += arg.ParameterType.Name + " " + arg.Name + ", ";
                }
                args = args.Trim().Trim(',') + ")";
                sb.AppendLine("\t" + GetMethodScope(mi) + "\t" + mi.ReturnType.Name + " " + mi.Name + args);
            }
            return methodInfos;
        }

        public static FieldInfo[] GetListOfFields(object obj, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            FieldInfo[] fields = null;
            Type type = obj.GetType();
            flags = Flags | flags;

            if (BaseType)
            {
                type = type.BaseType; // BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);//  BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            }

            fields = type.GetFields(flags);
            sb.AppendLine("List of private fields of the class [" + type.Name + "]:");
            foreach (FieldInfo field in fields)
            {
                sb.Append("\t" + GetFieldScope(field) + " " + field.FieldType.Name + "\t" + field.Name);
                object value = GetPrivateFieldValue(type, field.Name, obj, flags);
                if (value != null)
                {
                    sb.Append(" = " + value.ToString());
                }
                sb.AppendLine();
            }
            sb.AppendLine();
            sb.AppendLine("Change value of the private fields of the class " + type.Name + ":");
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.Name.ToLower() == "int32")
                {
                    bool state = SetPrivateFieldValue(obj, field.Name, -16101975, flags, BaseType);
                    if (state)
                    {
                        object value = GetPrivateFieldValue(type, field.Name, obj, flags);
                        if (value != null)
                        {
                            sb.AppendLine("\t" + GetFieldScope(field) + " " + field.FieldType.Name + "\t" + field.Name + " = " + value.ToString());
                        }
                    }
                }
            }

            return fields;
        }

        public static void ExecMethod(object obj, string MethodName, Object[] arguments, BindingFlags flags = BindingFlags.Default, bool BaseType = false)
        {
            //Type t = typeof(HotExchager_Solver.Form1);
            Type type = obj.GetType();
            flags = Flags | flags;

            if (BaseType)
            {
                type = type.BaseType; // BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);//  BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            }

            string args = string.Empty;
            foreach (object arg in arguments)
            {
                args += arg.ToString() + ", ";
            }

            args = args.Trim().TrimEnd(',');
            sb.AppendLine("Execute method: " + MethodName + "(" + args +")");
            MethodInfo m = type.GetMethod(MethodName, flags);
            if (m != null)
            {
                object result = m.Invoke(obj, arguments);
                sb.AppendLine("The method has returned: " + result.ToString());
            }
            else
            {
                sb.AppendLine("Couldn't found the method [" + MethodName + "]");
            }
        }

        public static void SubscribeEvent(object obj, Type control, Type typeHandler, string EventName, MethodInfo method, bool IsConsole = false)
        {
            sb.AppendLine("Subscribe event [" + EventName + "] to the method [" + method.Name + "] using the delegate [" + typeHandler.Name + "].");

            EventInfo eventInfo = control.GetEvent(EventName); //"Load"

            // Create the delegate on the test class because that's where the
            // method is. This corresponds with `new EventHandler(test.WriteTrace)`.
            //Type type = typeof(EventHandler);
            Delegate handler;
            if (IsConsole)
            {
                handler = Delegate.CreateDelegate(typeHandler, null, method);
                eventInfo.AddEventHandler(obj, handler);
            }
            else
            {
                handler = Delegate.CreateDelegate(typeHandler, obj, method);
                eventInfo.AddEventHandler(control, handler);
            }
        }

        public static void SubscribeEvent(object obj, Control control, Type typeHandler, string EventName, MethodInfo method)
        {
            if (typeof(Control).IsAssignableFrom(control.GetType()))
            {
                SubscribeEvent(obj, control.GetType(), typeHandler, EventName, method);
            }
        }

        public static void UnSubscribeEvent(object obj, Control control, string EventName, MethodInfo method)
        {
            if (typeof(Control).IsAssignableFrom(control.GetType()))
            {
                UnSubscribeEvent(obj, control.GetType(), EventName, method);
            }
        }

        public static void UnSubscribeEvent(object obj, Type control, string EventName, MethodInfo method)
        {
            if (obj != null)
            {
                EventInfo eventInfo = control.GetEvent(EventName);

                Type type = typeof(EventHandler);
                Delegate handler = Delegate.CreateDelegate(type, obj, method);

                // detach the event handler
                if (handler != null)
                    eventInfo.RemoveEventHandler(control, handler);
            }
        }
    }
}
