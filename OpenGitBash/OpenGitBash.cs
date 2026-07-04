// MIT License - Copyright (c) odvancee
// Source: https://github.com/odvancee/unity-tools

#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public static class OpenGitBash
{
	[DllImport("user32.dll")]
	private static extern bool ShowWindow(nint hWnd, int nCmdShow);

	[DllImport("user32.dll")]
	private static extern bool SetForegroundWindow(nint hWnd);

	private const int SW_RESTORE = 9;

	[MenuItem("Tools/Git Bash %g", false)]
	private static void FocusOrCreateWindow()
	{
		if (TryFocusExistingWindow())
		{
			return;
		}

		CreateWindow();
	}

	[MenuItem("Tools/Git Bash (New Window) %&g", false)]
	[MenuItem("Assets/Open Git Bash", false, 999)]
	private static void CreateWindow()
	{
		string projectRootPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
		string bashPath = ResolveGitBash();

		if (bashPath == null)
		{
			UnityEngine.Debug.LogError("Git Bash could not be resolved. Ensure Git is installed and added to PATH.");
			return;
		}

		Process.Start(new ProcessStartInfo
		{
			FileName = bashPath,
			WorkingDirectory = projectRootPath,
			UseShellExecute = true
		});
	}

	private static bool TryFocusExistingWindow()
	{
		Process[] processes = Process.GetProcessesByName("mintty");
		if (processes.Length == 0)
		{
			processes = Process.GetProcessesByName("git-bash");
		}

		if (processes.Length == 0)
		{
			return false;
		}

		nint hWnd = processes[0].MainWindowHandle;
		if (hWnd != 0)
		{
			ShowWindow(hWnd, SW_RESTORE);
			SetForegroundWindow(hWnd);
			return true;
		}

		return false;
	}

	private static string ResolveGitBash()
	{
		try
		{
			using (Process process = new Process())
			{
				process.StartInfo = new ProcessStartInfo()
				{
					FileName = "git",
					Arguments = "--exec-path",
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};

				process.Start();
				string execPath = process.StandardOutput.ReadLine();
				process.WaitForExit();

				if (string.IsNullOrEmpty(execPath))
				{
					return null;
				}

				string gitRoot = Directory.GetParent(execPath).Parent.Parent.FullName;

				string bashPath = Path.Combine(gitRoot, "git-bash.exe");
				return File.Exists(bashPath) ? bashPath : null;
			}
		}
		catch
		{
			return null;
		}
	}
}
#endif