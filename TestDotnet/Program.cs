﻿using System.Reflection;
using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");

NativeLibrary.SetDllImportResolver(typeof(Library).Assembly, Library.ImportResolver);
//NativeLibrary.TryLoad("libxphp.so", out var handle);
//Console.WriteLine("handle: {0}", handle);

const int PHP_SUCCESS = 0;
const string script_filename = "/Nightmare/TestCpp/main.php";

unsafe {
  var code2 = On(
          SendHeaderCallback,
          UbWriteCallback,
          FlushCallback,
          ReadPostCallback,
          ReadCookiesCallback,
          ServerParamCallback);

  if(code2 != PHP_SUCCESS) {
    Console.Error.WriteLine("XPhpScript: Error Code {0}", code2);
  }
}
var run = true;
var wait = new TaskCompletionSource<bool>();
var trd = new Task[100];
for (var i = trd.Length - 1; i >= 0; i--) {
	trd[i] = new Task(test);
  trd[i].Start();
}

while (run) {
	switch (Console.ReadKey().Key) {
	case ConsoleKey.A:
		for (var i = 1000; i > 0; i--) {
			_ = Task.Factory.StartNew(() => {
				var code = Execute(0, "GET", "", 0, 10, script_filename, "");
			});
		}
		continue;
	case ConsoleKey.B:
		for (var i = 1000; i > 0; i--) {
			var thread = new Thread(() => {
				var code = Execute(0, "GET", "", 0, 10, script_filename, "");
			});
			thread.Start();
		}
		continue;
	case ConsoleKey.Z:
		run = false;
		continue;
	}
	wait.SetResult(true);
	wait = new TaskCompletionSource<bool>();
}

void test() {
	while (run) {
    _ = wait.Task.Result;

		for (var i = 1000; i > 0; i--) {
			var code = Execute(context, "GET", "", 0, 10, script_filename, "");
		}
	}
}

Console.WriteLine("Main Thread");

var context = tsrm_thread_id();
Console.WriteLine("tid: {0}", context);

var code = Execute(context, "GET", "", 0, 10, script_filename, "");
Console.WriteLine("code: {0}", code);

Console.WriteLine("Multi Thread");

Task? wait = null;
for (var i = 5; i > 0; i--) {
  //wait = Task.Run(Test);
}

Task.WaitAll(
	Task.Run(Test),
	Task.Run(Test),
	Task.Run(Test),
	Task.Run(Test),
	Task.Run(Test)
);

Off();
Console.WriteLine("DONE");


void Test() {
  var context = tsrm_thread_id();
  Console.WriteLine("tid: {0}", context);

  var code = Execute(context, "GET", "", 0, 10, script_filename, "");
  Console.WriteLine("code: {0}", code);
}



void SendHeaderCallback(nuint thread_id, string head, long str_length) { 

}

unsafe long UbWriteCallback(nuint thread_id, string str, long str_length) {
	Console.Write(str);
	return str_length;
}

int FlushCallback(nuint thread_id) {
	Console.WriteLine("flush tid: {0}", thread_id);
	return 0;
}

unsafe long ReadPostCallback(nuint thread_id, byte* buf, long count_bytes) {
	Console.WriteLine("read tid: {0}", thread_id);
	return 0;
}

string ReadCookiesCallback(nuint thread_id) {
	Console.WriteLine("cookie tid: {0}", thread_id);
	return "e=";
}

void ServerParamCallback(nuint thread_id, nuint track_vars_array, ServerParamRetnCallback retn) {
  retn(track_vars_array, "DOCUMENT_ROOT", "/Nightmare/TestDotnet/");
  retn(track_vars_array, "HTTP_TEST", "123");
}



[DllImport("libxphp", CallingConvention = CallingConvention.Cdecl)]
static extern nuint tsrm_thread_id();
[DllImport("libxphp", CallingConvention = CallingConvention.Cdecl)]
static extern int On(
  SendHeader sendHeader,
  UbWrite write,
  Flush flush,
  ReadPost readPost,
  ReadCookies readCookies,
  ServerParam serverParam);
[DllImport("libxphp", CallingConvention = CallingConvention.Cdecl)]
static extern int Off();
[DllImport("libxphp", CallingConvention = CallingConvention.Cdecl)]
static extern int Execute(nuint context, string method, string content_type, long content_length, int filename_index, string filepath, string query_string);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ExecuteThreadIdCallback(nuint thread_id);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ServerParamRetnCallback(nuint track_vars_array, string key, string val);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void SendHeader(nuint thread_id, string head, long str_length);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
unsafe public delegate long UbWrite(nuint thread_id, string str, long str_length);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int Flush(nuint thread_id);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
unsafe public delegate long ReadPost(nuint thread_id, byte* buf, long count_bytes);
[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
public delegate string ReadCookies(nuint thread_id);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ServerParam(nuint thread_id, nuint track_vars_array, ServerParamRetnCallback retn);


static class Library {
  const string MyLibrary = "libxphp";

  public static IntPtr ImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath) {
    IntPtr libHandle = IntPtr.Zero;
    Console.WriteLine(libraryName);
    if(libraryName == MyLibrary) {
      NativeLibrary.TryLoad("libxphp.so", assembly, DllImportSearchPath.System32, out libHandle);
    }
    return libHandle;
  }
}