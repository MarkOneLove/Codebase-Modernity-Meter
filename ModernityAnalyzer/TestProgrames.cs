using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ModernityAnalyzer
{
    public class TestProgrames
    {

        // 7.1 Pattern-matching with generics
        public const string patternMatchingProgramme = @"
                    public class Example<T>
                    {
                        public void CheckType(T input)
                        {
                            if (input is int number)
                            {
                                Console.WriteLine($""Input is an integer with value {number}."");
                            }
                            else if (input is string)
                            {
                                Console.WriteLine($""Input is a string with value \""\"". "");
                            }
                            else
                            {
                                Console.WriteLine(""Input is of another type."");
                            }
                        }
                    }";

        // 7.1 Infer tuple names
        public const string inferredTupleNameProgramme = @"
            class Program
            {
                static (int x, int y) GetPoint()
                {
                    return (10, 20);
                }

                static void Main(string[] args)
                {
                    var point = GetPoint();
                    var (a, b) = point;

                    Console.WriteLine(a);
                    Console.WriteLine(b);
                }
            }
        ";

        // 7.1 Default expressions
        public const string defaultExpressionsProgramme = @"
            class Program
            {
                static void Main(string[] args)
                {
                    int defaultValue1 = default(int);
                    int defaultValue2 = default;

                    ImmutableArray<Integer> x = default;

                    var x = flag ? default : ImmutableArray<String>.Empty;
                    var x = new[] { default, ImmutableArray.Create(y) };

                    Console.WriteLine(defaultValue1);
                    Console.WriteLine(defaultValue2);
                }
                private int gg()
                {
                    return default;
                }
                void someMethod(ImmutableArray<Integer> arrayOpt = default)
            }
        ";

        // 7.1 Async Main
        public const string asyncMainProgramme = @"
            using System;
            using System.Net.Http;

            class Test
            {
                static async Task Main(string[] args) =>
	                Console.WriteLine(await new HttpClient().GetStringAsync(args[0]));
            }";

        // 7.2 private protected
        public const string privateProtectedPrograme =
                    @"using System;
                    using System.Collections;
                    using System.Linq;
                    using System.Text;

                    namespace HelloWorld
                    {
                        class Program
                        {
                            static void Main(string[] args)
                            {
                                private protected int myValue = 0;
                            }
                            private protected void GG()
                            {
                                Console.WriteLine(""BaseClass message."");
                            }
                        }
                    }";

        // 7.2 non-trailing named arguments
        public const string nontrailingNameArgsProgramme = @"
            public void DoSomething(bool isEmployed, string personName, int personAge) { }


            void M2()
            {
                DoSomething(isEmployed:true, name, age); // currently CS1738, but would become legal
                DoSomething(true, personName:name, age); // currently CS1738, but would become legal
                DoSomething(true, personAge:age, personName:name); // already legal
            }";

        // 7.2 Digit separator after base specifier
        public const string digitSeparatorAfterProgramme = @"
            class Program
            {
                static void Main(string[] args)
                {
                    int value1 = 0x_1_2;
                    int value2 = 0b_1_0_1;
                    int value3 = 1_000_000;
                    int value4 = 123;
                }
            }";

        // 7.2 Conditional ref expressions
        public const string conditionalRefExprProgramme = @"
            class Program
            {
                static void Main()
                {
                    int a = 10;
                    int b = 20;
                    ref var r = ref (arr != null ? ref arr[0]: ref otherArr[0]);
                    ref int refToAOrB = ref (a > b ? ref a : ref b);
                    int max = a > b ? a : b; // no ref
                }
            }";

        // 7.2 in keyword as a modifier in the parameter signature and in argument
        public const string inKeyInParamAndArgProgramme = @"
            class Example
            {
                void Method1(in int param1, int param2)
                {
                }

                int this[in int index]
                {
                    get { return index; }
                }

                public static Example operator +(in Example a, Example b)
                {
                    return new Example();
                }

                delegate void MyDelegate(in int param);

                void Method2()
                {
                    MyDelegate del = (in int param) => { };
                    void LocalFunction(in int param) { };
                    Method1(in param2);
                }
            }
        ";

        // 7.3 Unmanaged type constraint
        public const string unmanagedProgramme = @"
            public class Example<T> where T : unmanaged
            {
                public void Method<U>(U param) where U : unmanaged
                {
                }
            }
        ";

        // 7.3 Tuple == and != support
        public const string tupleOperatorsProgramme = @"
        class Example
            {
                void CompareTuples()
                {
                    var tuple1 = (1, ""apple"");
                    var tuple2 = (1, ""apple"");
                    var tuple3 = (2, ""orange"");

                    if (tuple1 == tuple2)
                    {
                        // Do something
                    }

                    if((1, ""apple"") == (2, ""orange"")) 
                    {
                    
                    }

                    if (tuple1 != tuple3)
                    {
                        // Do something else
                    }
                }
            }
        ";

        // 7.3 field attribute
        public const string fieldProgramme = @"
            using System;
            using System.Runtime.Serialization;

            [AttributeUsage(AttributeTargets.Field)]
            public class MyCustomAttribute : Attribute
            {
                public string Info { get; }

                public MyCustomAttribute(string info)
                {
                    Info = info;
                }
            }

            public class Example
            {
                [field: NonSerialized]
                public int AutoProperty { get; set; }

                [field: MyCustomAttribute(""This is a custom attribute"")]
                public string AnotherAutoProperty { get; set; }
            }
        ";

        // 7.3 stack alloc
        public const string stackAllocArrayProgramme = @"
            using System;
            class Example
            {
                unsafe static void Main(string[] args)
                {
                    int* p = stackalloc int[] { 1, 2, 3, 4 };
                }
            }
        ";

        // 7.3 fixed-sized buffers
        public const string fixSizeBufProgramme = @"
        unsafe struct MyStruct
        {
            public fixed int IntBuffer[10];
            public fixed bool BoolBuffer[10];   // New in C# 7.3
            public fixed char CharBuffer[10];   // New in C# 7.3
            public fixed short ShortBuffer[10]; // New in C# 7.3
            public fixed ushort UShortBuffer[10]; // New in C# 7.3
        }

        class Example
        {
            static unsafe void Main()
            {
                MyStruct myStruct;
            }
        }
        ";


        // 7.3 Expression variables in initializers
        public const string exprVarInInitProgramme = @"
        using System;

        class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        class Example
        {
            static void Main()
            {
                string name = ""John"";
                int age = 30;

                // Using expression variables in object initializer
                var person = new Person
                {
                    Name = name.ToUpper(),
                    Age = CalculateAge(age)
                };
            }

            static int CalculateAge(int age)
            {
                // Some calculation logic
                return age * 2;
            }
        }
        ";

        // 8.0 Readonly Instance Members
        public const string readonlyInstMemProgramme = @"
        public struct Point
        {
            public int X { get; }
            public int Y { get; }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public readonly double Distance => Math.Sqrt(X * X + Y * Y);

            public readonly override string ToString()
            {
                return $""({X}, {Y})"";
            }
        }
        ";

        // 8.0 Static local function
        public const string staticLocalFuncProgramme = @"
        using System;

        class Program
        {
            static void Main(string[] args)
            {
                int x = 10;
                int y = 20;

                // Non-static local function
                int AddNonStatic()
                {
                    return x + y;
                }

                // Static local function
                static int AddStatic(int a, int b)
                {
                    return a + b;
                }

                Console.WriteLine($""Non-static local function result: {AddNonStatic()}"");
                Console.WriteLine($""Static local function result: {AddStatic(x, y)}"");
            }
        }
        ";

        // 8.0 Unmanaged constructed types
        public const string unmanagedConstrTypes = @"
        using System;

        class Program
        {
            static void Main()
            {
                var point = new Point<int>(10, 20);
                Console.WriteLine(point);
            }
        }

        public struct Point<T> where T : unmanaged
        {
            public T X { get; }
            public T Y { get; }

            public Point(T x, T y)
            {
                X = x;
                Y = y;
            }

            public override string ToString() => $""({X}, {Y})"";
        }
        ";

        // 8.0 Stackalloc in nested contexts
        public const string stackallocInNestedProgramme = @"
           using System;

            class Program
            {
                static void Main()
                {
                    // Using stackalloc in a method call
                    Span<int> numbers = GetSpan(stackalloc int[] { 1, 2, 3, 4, 5 });
                    foreach (var number in numbers)
                    {
                        Console.WriteLine(number);
                    }

                    // Using stackalloc in a conditional expression
                    Span<int> conditionalNumbers = true ? stackalloc int[] { 6, 7, 8, 9, 10 } : stackalloc int[] { 11, 12, 13 };
                    foreach (var number in conditionalNumbers)
                    {
                        Console.WriteLine(number);
                    }
                }

                static Span<int> GetSpan(Span<int> span)
                {
                    return span;
                }
            }            
        ";

        // 8.0 Alternative interpolated verbatim strings
        public const string newStringsProgramme = @"
        using System;

        class Program
        {
            static void Main()
            {
                string name = ""World"";
                string greeting1 = $@""Hello, {name}!"";
                string greeting2 = @$""Hello, {name}!"";
            }
        }
        ";

        // 8.0 Null-coalescing Assignment
        public const string nullCoalAssProgramme = @"
        using System;

        class Program
        {
            static void Main()
            {
                string message = null;
                message ??= ""Hello, World!"";
            }
        }
        ";

        // 8.0 Ranges
        public const string rangeProgramme = @"
        using System;

        class Program
        {
            static void Main()
            {
                int[] numbers = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        
                // Using range expressions
                var slice1 = numbers[2..5];
                var slice2 = numbers[3..^2];
        
                // Using Index class
                Index startIndex = ^5;
                var slice3 = numbers[startIndex..];
            }
        }
        ";

        // 8.0 Enhanced using: "pattern-based using" and "using declarations"
        public const string enahncedUsingProgramme = @"
        using System;

        class Program
        {
            static Resource AcquireResource() => new Resource();
            
            static void Main()
            {
                // Pattern-based using
                using var resource = AcquireResource();

                using (var r = new Resource())
                {
                    int a = 4;
                }

                if (resource is Resource && true) // Some condition
                {
                    Console.WriteLine(""Resource is valid and used"");
                }

                // Using declaration
                using Resource resource2 = AcquireResource();
                Console.WriteLine(""Resource 2 used"");
            }

        }

        class Resource : IDisposable
        {
            public void Dispose()
            {
                Console.WriteLine(""Resource disposed"");
            }
        }
        ";

        // 8.0 Async Streams
        public const string asyncStreamsProgramme = @"
        using System;
        using System.Collections.Generic;
        using System.Threading.Tasks;

        class Program
        {
            static async Task Main()
            {
                await foreach (var number in GetNumbersAsync())
                {
                    Console.WriteLine(number);
                }
            }

            static async IAsyncEnumerable<int> GetNumbersAsync()
            {
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(100);
                    yield return i;
                }
            }
        }
        ";

        // 8.0 Recursive Pattern Matching
        public const string recPatMatchProgramme = @"
        using System;

        class Program
        {
            public static void Main()
            {
                object shape = new Point(2, 3);

                // Recursive pattern matching in switch statement
                switch (shape)
                {
                    case Point (0, 0):
                        Console.WriteLine(""Origin"");
                        break;
                    case Point (int x, 0):
                        Console.WriteLine($""On X-axis at {x}"");
                        break;
                    case Point (0, int y):
                        Console.WriteLine($""On Y-axis at {y}"");
                        break;
                    case Point (int x, int y):
                        Console.WriteLine($""At ({x}, {y})"");
                        break;
                    default:
                        Console.WriteLine(""Unknown shape"");
                        break;
                }
            }
        }

        public class Point
        {
            public int X { get; }
            public int Y { get; }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);
        }
        }
        ";

        // 8.0 default interface
        public const string defaultInterfaceProgramme = @"
        using System;

        interface ILogger
        {
            // Default method implementation in the interface
            void Log(string message)
            {
                Console.WriteLine($""Default log: {message}"");
            }

            // Abstract method - must be implemented by the implementing class
            void LogError(string message);
        }

        class ConsoleLogger : ILogger
        {
            // Implementing the abstract method
            public void LogError(string message)
            {
                Console.WriteLine($""Error: {message}"");
            }

            // Optionally override the default method
            public void Log(string message)
            {
                Console.WriteLine($""Console log: {message}"");
            }
        }

        class Program
        {
            static void Main()
            {
                ILogger logger = new ConsoleLogger();
                logger.Log(""This is a log message."");
                logger.LogError(""This is an error message."");
            }
        }
        ";

        // 9.0 Target-typed new expression
        public const string targTypeNewProgramme = @"
        using System;
        using System.Collections.Generic;

        public class Point
        {
            public int X { get; }
            public int Y { get; }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public override string ToString() => $""({X}, {Y})"";
        }

        public class Program
        {
            public static void Main()
            {
                Point p1 = new(3, 4);
                Point p2 = CreatePoint();
                PrintPoint(new(5, 6));
                List<Point> points = new() { new(7, 8), new(9, 10) };
            }

            public static Point CreatePoint() => new(1, 2);

            public static void PrintPoint(Point p) => Console.WriteLine(p);
        }
        ";

        // 9.0 SkipLocalsInit
        public const string skipLocInitProgramme = @"
        using System;
        using System.Runtime.CompilerServices;

        [assembly: SkipLocalsInit]

        public class Program
        {
            [SkipLocalsInit]
            public static void Main()
            {
                int x;
                Console.WriteLine(x);
            }

            [SkipLocalsInit]
            public static void MethodLevel()
            {
                int x;
                Console.WriteLine(x);
            }
        }

        [SkipLocalsInit]
        public class ClassLevel
        {
            public static void Method1()
            {
                int x;
                Console.WriteLine(x);
            }

            public static void Method2()
            {
                int y;
                Console.WriteLine(y);
            }
        }
        ";

        // 9.0 _ in lambda functions
        public const string discardInLambdaProgramme = @"
        using System;

        public class Program
        {
            public static void Main()
            {
                // Using a discard parameter in a lambda expression
                Action<int, int> action = (_, y) => Console.WriteLine($""Only using the second parameter: {y}"");
                action(1, 2);

                // Using multiple discard parameters
                Func<int, int, int, int> func = (_, _, z) => z * 2;
                Console.WriteLine(func(1, 2, 3));
            }
        }
        ";

        // 9.0 Native-sized integers
        public const string nativeSizeIntProgramme = @"
        using System;

        public class Example
        {
            public void Method()
            {
                nint nativeInt = 42;
                nuint nativeUInt = 42U;
                Func<int, int, int, int> func = (_, _, z) => z * 2;
            }
        }
        ";

        // 9.0 Attributes on local functions
        public const string localFuncAttrProgramme = @"
        using System;
        using System.Runtime.CompilerServices;

        public class Program
        {
            public static void Main()
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void LocalFunction()
                {
                    Console.WriteLine(""This is a local function with an attribute."");
                }

                LocalFunction();
            }
        }
        ";

        // 9.0 Function pointers
        public const string funcPointerProgramme = @"
        using System;
        using System.Runtime.InteropServices;

        public class Program
        {
            public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

            public static void Main()
            {
                delegate* unmanaged<IntPtr, string, string, uint, int> msgBoxPtr = &MessageBox;
                msgBoxPtr(IntPtr.Zero, ""Hello, world!"", ""Function Pointer"", 0);
            }
        }
        ";

        // 9.0 Pattern matching improvements
        public const string patterMatchImprProgramme = @"
        using System;

        public class Program
        {
            public static void Main()
            {
                object obj = (3, 4);
        
                if (obj is (int x, int y))
                {
                    Console.WriteLine($""Tuple with values: ({x}, {y})"");
                }

                int number = 42;
                if (number is > 0 and < 100)
                {
                    Console.WriteLine(""Number is between 0 and 100."");
                }

                if (number is >= 10 and <= 50)
                {
                    Console.WriteLine(""Number is between 10 and 50."");
                }

                var point = new Point(3, 4);
                if (point is Point(3, 4))
                {
                    Console.WriteLine(""Point is at (3, 4)."");
                }

                var person = new Person(""John"", ""Doe"");
                if (person is { FirstName: ""John"", LastName: ""Doe"" })
                {
                    Console.WriteLine(""Person is John Doe."");
                }
            }

            public record Point(int X, int Y);
            public record Person(string FirstName, string LastName);
        }
        ";

        // 9.0 Static lambdas
        public const string statLambdaProgramme = @"
        using System;

        public class Program
        {
            public static void Main()
            {
                int value = 42;
                Func<int, int> regularLambda = x => x + value;
                Func<int, int> staticLambda = static x => x * 2;

                Console.WriteLine(regularLambda(5));
                Console.WriteLine(staticLambda(5));
            }
        }
        ";

        // 9.0 Records
        public const string recordProgramme = @"
        public record Person(string FirstName, string LastName);
        public record Employee(string FirstName, string LastName, string EmployeeId) : Person(FirstName, LastName);
        ";

        // 9.0 Target-Typed Conditional Expression
        public const string targTypeExprProgramme = @"
        using System;

        public class Program
        {
            public static void Main()
            {
                string result1 = true ? ""Hello"" : ""World"";
                var result2 = true ? ""Hello"" : ""World"";\
                ref var r = ref (arr != null ? ref arr[0]: ref otherArr[0]);
                ref int refToAOrB = ref (a > b ? ref a : ref b);
                int max = a > b ? a : b; // no ref
            }
        }
        ";

        // 9.0 Covariant returns
        public const string covariantReturnsProgramme = @"
        using System;

        public class Program
        {
            public static void Main()
            {
                Animal cat = new Cat();
                Console.WriteLine(cat.MakeSound());
            }
        }

        public class Animal
        {
            public virtual string MakeSound() => ""Generic animal sound"";
        }

        public class Cat : Animal
        {
            public override string MakeSound() => ""Meow"";
        }
        ";

        // 9.0 Extension GetEnumerator
        public const string extGetEnumProgramme = @"
        using System;
        using System.Collections.Generic;

        public static class MyExtensions
        {
            public static IEnumerator<T> GetEnumerator<T>(this MyClass<T> collection)
            {
                return collection.GetEnumerator();
            }
        }

        public class MyClass<T>
        {
            private List<T> items = new List<T>();

            public void Add(T item)
            {
                items.Add(item);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return items.GetEnumerator();
            }
        }

        class Program
        {
            static void Main(string[] args)
            {
                var myClass = new MyClass<int>();
                myClass.Add(1);
                myClass.Add(2);
                int[] array2 = [1, 2, 3, 4, 5, 6];

                foreach (var item in myClass)
                {
                    Console.WriteLine(item);
                }
                foreach (var numba in array2)
                {
                    Console.WriteLine(numba);
                }
            }
        }
        ";

        // 9.0 Module initializers
        public const string modInitProgramme = @"
        using System;
        using System.Runtime.CompilerServices;

        public static class MyModuleInitializer
        {
            [ModuleInitializer]
            public static void Initialize()
            {
                Console.WriteLine(""Module initialized"");
            }
        }
        ";

        // 9.0 Extending Partial Methods
        public const string extPartMethodProgramme = @"
        partial class MyClass
        {
            partial void PartialMethod();
        }

        partial class MyClass
        {
            partial void PartialMethod()
            {
                Console.WriteLine(""Partial method extended"");
            }
        }
        ";

        // 9.0 Top level statements
        public const string topLvlStatementsProgramme = @"
        using System;

        Console.WriteLine(""Hello, world!"");
        ";

        // 10.0 record struct declaration
        public const string recStructProgramme = @"
        global using System;
        global using System.Collections.Generic;
        global using ProjectSpecificAlias = SomeNamespace.SomeType;
        
        public record struct Point(int X, int Y);

        class Program
        {
            static void Main()
            {
                var p1 = new Point(1, 2);
                var p2 = new Point(1, 2);
        
                Console.WriteLine(p1 == p2);  // True
                Console.WriteLine(p1.Equals(p2));  // True
            }
        }
        ";

        // 10.0 Constant interpolated strings
        public const string constInterStrProgramme = @"
        public class Example
        {
            const string Name = ""World"";
            const string Greeting = $""Hello, {Name}!"";
        }
        ";

        // 10.0 Extended property patterns
        public const string extPropPatProgramme = @"
        public class Address
        {
            public string City { get; set; }
        }

        public class Person
        {
            public string Name { get; set; }
            public Address Address { get; set; }
        }

        public class Example
        {
            public void CheckPerson(Person person)
            {
                // Using extended property patterns
                if (person is { Address.City: ""Seattle"" })
                {
                    Console.WriteLine($""{person.Name} lives in Seattle."");
                }
            }
        }
        ";

        // 10.0 Records with sealed base toString override
        public const string recWithSealProgramme = @"
        public record BaseRecord(string Name)
        {
            public sealed override string ToString() => $""BaseRecord: {Name}"";
        }

        public record DerivedRecord(string Name, int Age) : BaseRecord(Name);
        ";

        // 10.0 Mix Declarations and Variables in Deconstruction
        public const string mixDeclProgramme = @"
        class Program
        {
            static void Main()
            {
                int x = 1;
                (x, int y) = (2, 3);
            }
        }
        ";

        // 10.0 Allow [AsyncMethodBuilder(...)] on methods
        public const string allowAsyncMethodProgramme = @"
        using System;
        using System.Runtime.CompilerServices;
        using System.Threading.Tasks;

        [AsyncMethodBuilder(typeof(CustomAsyncMethodBuilder<>))]
        public class CustomAsyncMethodBuilder<T>
        {
            public static CustomAsyncMethodBuilder<T> Create() => new CustomAsyncMethodBuilder<T>();

            public void SetStateMachine(IAsyncStateMachine stateMachine) { }

            public void SetResult(T result) { }

            public void SetException(Exception exception) { }

            public Task<T> Task => Task.FromResult(default(T));

            public void Start<TStateMachine>(ref TStateMachine stateMachine)
                where TStateMachine : IAsyncStateMachine
            {
                stateMachine.MoveNext();
            }

            public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
                where TAwaiter : INotifyCompletion
                where TStateMachine : IAsyncStateMachine
            {
                stateMachine.MoveNext();
            }

            public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
                where TAwaiter : ICriticalNotifyCompletion
                where TStateMachine : IAsyncStateMachine
            {
                stateMachine.MoveNext();
            }
        }

        public class Example
        {
            [AsyncMethodBuilder(typeof(CustomAsyncMethodBuilder<>))]
            public async Task<int> CustomAsyncMethod()
            {
                await Task.Delay(1000);
                return 42;
            }
        }
        ";

        // 10.0 Enhanced #line directives
        public const string lineProgramme = @"
        using System;

        class Program
        {
            static void Main()
            {
                Console.WriteLine(""""Line 1"""");

        #line 100 """"CustomFileName.cs""""
                Console.WriteLine(""""Line 100 in CustomFileName.cs"""");

        #line hidden
                Console.WriteLine(""""This line is hidden in the debugger"""");

        #line default
                Console.WriteLine(""""Back to actual line numbers and file names"""");
            }
        }    
        ";

        // 10.0 Static abstract members in interfaces
        public const string staticAbstractProgramme = @"
        interface IExample<T>
        {
            static abstract T CreateInstance();
        }

        class Example : IExample<Example>
        {
            public static Example CreateInstance()
            {
                return new Example();
            }
        }
        ";

        // 10.0 Lambda improvements
        public const string imprLambdaProgramme = @"
        using System;

        class Program
        {
            static void Main()
            {
                Func<int, int> square = [MyAttribute] (int x) => { return x * x; };
                var add = (int x, int y) => x + y;
            }
        }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Delegate)]
        public class MyAttribute : Attribute
        {
        }
        ";

        // 10.0 File-scoped namespace
        public const string fileNamespaceProgramme = @"
        namespace MyNamespace;

        class Program
        {
            static void Main()
            {
                Console.WriteLine(""Hello, World!"");
            }
        }
        ";

        // 10.0 Parameterless struct constructors
        public const string pLessStructProgramme = @"
        public struct Point
        {
            public int X;
            public int Y;

            public Point()
            {
                X = -1;
                Y = -1;
            }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
        ";

        // 10.0 Caller expression attribute
        public const string callExprAtrProgramme = @"
        using System;
        using System.Runtime.CompilerServices;

        class Program
        {
            static void Main()
            {
                int value = 42;
                Validate(value > 50);
            }

            static void Validate(bool condition, [CallerArgumentExpression(""condition"")] string conditionExpression = null)
            {
                if (!condition)
                {
                    Console.WriteLine($""Condition failed: {conditionExpression}"");
                }
            }
        }
        ";

        // 11.0 File-local types
        public const string fileLocTypeProgramme = @"
        file class Helper
        {
            public static void DoSomething()
            {
                Console.WriteLine(""Doing something..."");
            }
        }

        public class Program
        {
            public static void Main()
            {
                Helper.DoSomething();
            }
        }
        ";

        // 11.0 ref fields
        public const string refFieldsProgramme = @"
        public ref struct RefStructExample
        {
            private ref int _refField;

            public RefStructExample(ref int refField)
            {
                _refField = ref refField;
            }

            public void SetValue(int value)
            {
                _refField = value;
            }
        }
        ";

        // 11.0 Required members
        public const string reqMemProgramme = @"
        public class Person
        {
            public required string FirstName { get; init; }
            public required string LastName { get; init; }
            public int Age { get; set; }

            public Person()
            {
                // Optionally, you can provide default values or leave it empty
            }
        }
        ";

        // 11.0 Unsigned Right Shift
        public const string unRightShiftProgramme = @"
        using System;

        class Program
        {
            static void Main()
            {
                int negativeValue = -8;
                uint positiveValue = 8;

                int shiftedNegative = negativeValue >>> 2;
                uint shiftedPositive = positiveValue >>> 2;
            }
        }
        ";

        // 11.0 Utf8 String Literals
        public const string utf8StrLitProgramme = @"
        using System;

        class Program
        {
            static void Main()
            {
                // UTF-8 string literal
                var utf8Literal = ""example""u8;
            }
        }
        ";

        // 11.0 Pattern matching on ReadOnlySpan<char>
        public const string patMatReadOnlyProgramme = @"
        using System;

        class Program
        {
            static void Main()
            {
                ReadOnlySpan<char> input = ""example"".AsSpan();

                switch (input)
                {
                    case ""example"":
                        Console.WriteLine(""Matched 'example'"");
                        break;
                    case ""test"":
                        Console.WriteLine(""Matched 'test'"");
                        break;
                    default:
                        Console.WriteLine(""No match"");
                        break;
                }
            }
        }
        ";

        // 11.0 Checked Operators
        public const string checkOpProgramme = @"
        using System;

        public struct SafeInt
        {
            private int value;

            public SafeInt(int value)
            {
                this.value = value;
            }

            // Custom checked addition operator
            public static SafeInt operator checked +(SafeInt a, SafeInt b)
            {
                return new SafeInt(checked(a.value + b.value));
            }

            // Custom unchecked addition operator
            public static SafeInt operator unchecked +(SafeInt a, SafeInt b)
            {
                return new SafeInt(unchecked(a.value + b.value));
            }

            public override string ToString() => value.ToString();
        }
        ";

        // 11.0 auto-default structs
        public const string autoDefStructProgramme = @"
        using System;

        struct MyStruct
        {
            public int Number;
            public string Text;
        }
        ";

        // 11.0 Newlines in interpolations
        public const string newlnInterpProgramme = @"
        using System;

        class Program
        {
            static void Main()
            {
                int a = 1;
                int b = 2;
                string result = $@""Values:\n
            a: {a}
            b: {b}"";
                Console.WriteLine(result);
            }
        }
        ";

        // 11.0 List patterns
        public const string listPatProgramme = @"
        using System;

        class Program
        {
            static void Main()
            {
                int[] numbers = { 1, 2, 3, 4, 5 };

                string result = numbers switch
                {
                    [1, 2, 3, 4, 5] => ""Exact match"",
                    [1, .., 5] => ""Starts with 1 and ends with 5"",
                    [1, ..] => ""Starts with 1"",
                    [.., 5] => ""Ends with 5"",
                    [_, _, 3, _, _] => ""Third element is 3"",
                    _ => ""No match""
                };

                Console.WriteLine(result);
            }
        }
        ";

        // 11.0 raw string literals
        public const string rawStrLitProgramme = @"
        using System;

        class Program
        {
            static void Main()
            {
                string rawString = """"""
                This is a raw string literal.
                It spans multiple lines
                and preserves whitespace and indentation.
                """""";

                int x = 10;
                int y = 20;
                string interpolatedRawString = $""""""
                The values are:
                x = {x}
                y = {y}
                """";
            }
        }
        ";

        // 11.0 nameof(parameter)
        public const string nameOfParamProgramme = @"
        using System;

        [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
        sealed class ExampleAttribute : Attribute
        {
            public ExampleAttribute(string parameterName)
            {
                ParameterName = parameterName;
            }

            public string ParameterName { get; }
        }

        class Program
        {
            [Example(nameof(parameter))]
            static void MyMethod(string parameter)
            {
                Console.WriteLine($""Method parameter name: {nameof(parameter)}"");
            }

            static void Main()
            {
                MyMethod(""test"");
            }
        }
        ";

        // 11.0 Generic attributes
        public const string genAttrProgramme = @"
           using System;

        [GenericAttribute<int>(""This is a generic attribute for an integer type."")]
        public class ExampleClass
        {
            [GenericAttribute<string>(""This is a generic attribute for a string type."")]
            public void ExampleMethod()
            {
                // Method implementation
            }
        } 
        ";

        // 12.0 ref readonly parameters
        public const string refReadParamProgramme = @"
        public partial class ExampleClass
        {
            public partial void ProcessData(ref readonly int data);
        }

        public partial class ExampleClass
        {
            public partial void ProcessData(ref readonly int data)
            {
                Console.WriteLine($""Processing data: {data}"");
            }
        }
        ";

        // 12.0 Collection expressions
        public const string colExprProgramme = @"
        using System;
        using System.Collections.Generic;

        class Program
        {
            static void Main()
            {
                List<int> numbers = [1, 2, 3, 4, 5];
                Dictionary<string, int> nameToAge = [""Alice"": 30, ""Bob"": 25, ""Charlie"": 35];
            }
        }
        ";

        // 12.0 Inline arrays
        public const string inlnArrProgramme = @"
        using System;
        using System.Runtime.CompilerServices;

        [InlineArray(5)]
        public struct InlineArray5
        {
            public int Element0;
            public int Element1;
            public int Element2;
            public int Element3;
            public int Element4;
        }

        public struct ExampleStruct
        {
            public InlineArray5 Numbers;
        }
        ";

        // 12.0 nameof accessing instance members
        public const string nameofInstProgramme = @"
        using System;

        public class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            private int age;

            public void PrintNames()
            {
                Console.WriteLine(nameof(FirstName)); // Outputs: FirstName
                Console.WriteLine(nameof(LastName));  // Outputs: LastName
                Console.WriteLine(nameof(age));       // Outputs: age
                Console.WriteLine(nameof(PrintNames)); // Outputs: PrintNames
            }
        }

        class Program
        {
            static void Main()
            {
                Person person = new Person();
                person.PrintNames();
            }
        }
        ";

        // 12.0 Using aliases for any type
        public const string aliasProgrammme = @"
        using System;
        using MyList = System.Collections.Generic.List<int>;
        using Point = System.Drawing.Point;
        using ConfigDictionary = System.Collections.Generic.Dictionary<string, string>;

        class Program
        {
            static void Main()
            {
                MyList numbers = new MyList { 1, 2, 3, 4, 5 };
                Point point = new Point(10, 20);
                ConfigDictionary config = new ConfigDictionary
                {
                    { ""Setting1"", ""Value1"" },
                    { ""Setting2"", ""Value2"" }
                };
            }
        }
        ";

        // 12.0 Primary Constructors
        public const string primConstrProgramme = @"
        public class Person(string firstName, string lastName)
        {
            public string FirstName { get; } = firstName;
            public string LastName { get; } = lastName;

            public void PrintFullName()
            {
                Console.WriteLine($""{FirstName} {LastName}"");
            }
        }

        public struct Point(int x, int y)
        {
            public int X { get; } = x;
            public int Y { get; } = y;

            public void PrintCoordinates()
            {
                Console.WriteLine($""({X}, {Y})"");
            }
        }
        ";

        // 12.0 Lambda optional parameters
        public const string lambdaOptProgramme = @"
        using System;

        class Program
        {
            static void Main()
            {
                // Lambda expression with an optional parameter
                Func<int, int, int> add = (a, b = 10) => a + b;
                Console.WriteLine(add(5, 20)); // Outputs: 25
                Console.WriteLine(add(5)); // Outputs: 15 (since b defaults to 10)
            }
        }
        ";

        // 12.0 Experimental attr
        public const string exprAttrProgramme = @"
        using System;
        using System.Diagnostics.CodeAnalysis;

        namespace ExperimentalFeatures
        {
            [Experimental(""This class is experimental and may change in future versions."")]
            public class ExperimentalClass
            {
                [Experimental(""This method is experimental and may change in future versions."")]
                public void ExperimentalMethod()
                {
                    Console.WriteLine(""This is an experimental method."");
                }
            }

            class Program
            {
                static void Main()
                {
                    var experimentalClass = new ExperimentalClass();
                    experimentalClass.ExperimentalMethod();
                }
            }
        }
        ";
    }
}
