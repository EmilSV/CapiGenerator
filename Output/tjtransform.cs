using System;
namespace TestProject;
public unsafe struct tjtransform
{
	public TestProject.tjregion r;
	public int op;
	public int options;
	public void* data;
	public delegate* unmanaged[Cdecl]<short*, TestProject.tjregion, TestProject.tjregion, int, int, TestProject.tjtransform*, int> customFilter;
}
