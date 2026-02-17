using System.Runtime.CompilerServices;
using UnityEngine;

// Deze assembly heet "UnityEngine.dll" (via AssemblyName in csproj)
// en forwardt de types naar de echte Unity 6 modules.
[assembly: TypeForwardedTo(typeof(UnityEngine.MonoBehaviour))]
[assembly: TypeForwardedTo(typeof(UnityEngine.Rect))]
[assembly: TypeForwardedTo(typeof(UnityEngine.Screen))]
[assembly: TypeForwardedTo(typeof(UnityEngine.GUI))]