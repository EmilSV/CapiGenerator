using System;
namespace TestProject;
public unsafe static class TestProjectFunction
{
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3Init", CallingConvention = CallingConvention.Cdecl)]
	public static extern TestProject.tjhandle tj3Init(int initType);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3Set", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3Set(TestProject.tjhandle handle,int param,int value);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3Get", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3Get(TestProject.tjhandle handle,int param);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3Compress8", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3Compress8(TestProject.tjhandle handle,byte* srcBuf,int width,int pitch,int height,int pixelFormat,byte** jpegBuf,nuint* jpegSize);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3Compress12", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3Compress12(TestProject.tjhandle handle,short* srcBuf,int width,int pitch,int height,int pixelFormat,byte** jpegBuf,nuint* jpegSize);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3Compress16", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3Compress16(TestProject.tjhandle handle,ushort* srcBuf,int width,int pitch,int height,int pixelFormat,byte** jpegBuf,nuint* jpegSize);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3CompressFromYUV8", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3CompressFromYUV8(TestProject.tjhandle handle,byte* srcBuf,int width,int align,int height,byte** jpegBuf,nuint* jpegSize);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3CompressFromYUVPlanes8", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3CompressFromYUVPlanes8(TestProject.tjhandle handle,byte** srcPlanes,int width,int* strides,int height,byte** jpegBuf,nuint* jpegSize);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3JPEGBufSize", CallingConvention = CallingConvention.Cdecl)]
	public static extern nuint tj3JPEGBufSize(int width,int height,int jpegSubsamp);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3YUVBufSize", CallingConvention = CallingConvention.Cdecl)]
	public static extern nuint tj3YUVBufSize(int width,int align,int height,int subsamp);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3YUVPlaneSize", CallingConvention = CallingConvention.Cdecl)]
	public static extern nuint tj3YUVPlaneSize(int componentID,int width,int stride,int height,int subsamp);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3YUVPlaneWidth", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3YUVPlaneWidth(int componentID,int width,int subsamp);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3YUVPlaneHeight", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3YUVPlaneHeight(int componentID,int height,int subsamp);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3EncodeYUV8", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3EncodeYUV8(TestProject.tjhandle handle,byte* srcBuf,int width,int pitch,int height,int pixelFormat,byte* dstBuf,int align);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3EncodeYUVPlanes8", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3EncodeYUVPlanes8(TestProject.tjhandle handle,byte* srcBuf,int width,int pitch,int height,int pixelFormat,byte** dstPlanes,int* strides);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3DecompressHeader", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3DecompressHeader(TestProject.tjhandle handle,byte* jpegBuf,nuint jpegSize);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3GetScalingFactors", CallingConvention = CallingConvention.Cdecl)]
	public static extern TestProject.tjscalingfactor* tj3GetScalingFactors(int* numScalingFactors);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3SetScalingFactor", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3SetScalingFactor(TestProject.tjhandle handle,TestProject.tjscalingfactor scalingFactor);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3SetCroppingRegion", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3SetCroppingRegion(TestProject.tjhandle handle,TestProject.tjregion croppingRegion);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3Decompress8", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3Decompress8(TestProject.tjhandle handle,byte* jpegBuf,nuint jpegSize,byte* dstBuf,int pitch,int pixelFormat);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3Decompress12", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3Decompress12(TestProject.tjhandle handle,byte* jpegBuf,nuint jpegSize,short* dstBuf,int pitch,int pixelFormat);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3Decompress16", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3Decompress16(TestProject.tjhandle handle,byte* jpegBuf,nuint jpegSize,ushort* dstBuf,int pitch,int pixelFormat);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3DecompressToYUV8", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3DecompressToYUV8(TestProject.tjhandle handle,byte* jpegBuf,nuint jpegSize,byte* dstBuf,int align);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3DecompressToYUVPlanes8", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3DecompressToYUVPlanes8(TestProject.tjhandle handle,byte* jpegBuf,nuint jpegSize,byte** dstPlanes,int* strides);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3DecodeYUV8", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3DecodeYUV8(TestProject.tjhandle handle,byte* srcBuf,int align,byte* dstBuf,int width,int pitch,int height,int pixelFormat);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3DecodeYUVPlanes8", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3DecodeYUVPlanes8(TestProject.tjhandle handle,byte** srcPlanes,int* strides,byte* dstBuf,int width,int pitch,int height,int pixelFormat);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3Transform", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3Transform(TestProject.tjhandle handle,byte* jpegBuf,nuint jpegSize,int n,byte** dstBufs,nuint* dstSizes,TestProject.tjtransform* transforms);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3Destroy", CallingConvention = CallingConvention.Cdecl)]
	public static extern void tj3Destroy(TestProject.tjhandle handle);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3Alloc", CallingConvention = CallingConvention.Cdecl)]
	public static extern void* tj3Alloc(nuint bytes);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3LoadImage8", CallingConvention = CallingConvention.Cdecl)]
	public static extern byte* tj3LoadImage8(TestProject.tjhandle handle,byte* filename,int* width,int align,int* height,int* pixelFormat);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3LoadImage12", CallingConvention = CallingConvention.Cdecl)]
	public static extern short* tj3LoadImage12(TestProject.tjhandle handle,byte* filename,int* width,int align,int* height,int* pixelFormat);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3LoadImage16", CallingConvention = CallingConvention.Cdecl)]
	public static extern ushort* tj3LoadImage16(TestProject.tjhandle handle,byte* filename,int* width,int align,int* height,int* pixelFormat);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3SaveImage8", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3SaveImage8(TestProject.tjhandle handle,byte* filename,byte* buffer,int width,int pitch,int height,int pixelFormat);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3SaveImage12", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3SaveImage12(TestProject.tjhandle handle,byte* filename,short* buffer,int width,int pitch,int height,int pixelFormat);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3SaveImage16", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3SaveImage16(TestProject.tjhandle handle,byte* filename,ushort* buffer,int width,int pitch,int height,int pixelFormat);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3Free", CallingConvention = CallingConvention.Cdecl)]
	public static extern void tj3Free(void* buffer);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3GetErrorStr", CallingConvention = CallingConvention.Cdecl)]
	public static extern byte* tj3GetErrorStr(TestProject.tjhandle handle);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tj3GetErrorCode", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tj3GetErrorCode(TestProject.tjhandle handle);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "TJBUFSIZE", CallingConvention = CallingConvention.Cdecl)]
	public static extern uint TJBUFSIZE(int width,int height);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjCompress", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjCompress(TestProject.tjhandle handle,byte* srcBuf,int width,int pitch,int height,int pixelSize,byte* dstBuf,uint* compressedSize,int jpegSubsamp,int jpegQual,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjDecompress", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjDecompress(TestProject.tjhandle handle,byte* jpegBuf,uint jpegSize,byte* dstBuf,int width,int pitch,int height,int pixelSize,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjDecompressHeader", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjDecompressHeader(TestProject.tjhandle handle,byte* jpegBuf,uint jpegSize,int* width,int* height);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjDestroy", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjDestroy(TestProject.tjhandle handle);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjGetErrorStr", CallingConvention = CallingConvention.Cdecl)]
	public static extern byte* tjGetErrorStr();
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjInitCompress", CallingConvention = CallingConvention.Cdecl)]
	public static extern TestProject.tjhandle tjInitCompress();
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjInitDecompress", CallingConvention = CallingConvention.Cdecl)]
	public static extern TestProject.tjhandle tjInitDecompress();
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "TJBUFSIZEYUV", CallingConvention = CallingConvention.Cdecl)]
	public static extern uint TJBUFSIZEYUV(int width,int height,int jpegSubsamp);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjDecompressHeader2", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjDecompressHeader2(TestProject.tjhandle handle,byte* jpegBuf,uint jpegSize,int* width,int* height,int* jpegSubsamp);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjDecompressToYUV", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjDecompressToYUV(TestProject.tjhandle handle,byte* jpegBuf,uint jpegSize,byte* dstBuf,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjEncodeYUV", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjEncodeYUV(TestProject.tjhandle handle,byte* srcBuf,int width,int pitch,int height,int pixelSize,byte* dstBuf,int subsamp,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjAlloc", CallingConvention = CallingConvention.Cdecl)]
	public static extern byte* tjAlloc(int bytes);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjBufSize", CallingConvention = CallingConvention.Cdecl)]
	public static extern uint tjBufSize(int width,int height,int jpegSubsamp);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjBufSizeYUV", CallingConvention = CallingConvention.Cdecl)]
	public static extern uint tjBufSizeYUV(int width,int height,int subsamp);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjCompress2", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjCompress2(TestProject.tjhandle handle,byte* srcBuf,int width,int pitch,int height,int pixelFormat,byte** jpegBuf,uint* jpegSize,int jpegSubsamp,int jpegQual,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjDecompress2", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjDecompress2(TestProject.tjhandle handle,byte* jpegBuf,uint jpegSize,byte* dstBuf,int width,int pitch,int height,int pixelFormat,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjEncodeYUV2", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjEncodeYUV2(TestProject.tjhandle handle,byte* srcBuf,int width,int pitch,int height,int pixelFormat,byte* dstBuf,int subsamp,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjFree", CallingConvention = CallingConvention.Cdecl)]
	public static extern void tjFree(byte* buffer);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjGetScalingFactors", CallingConvention = CallingConvention.Cdecl)]
	public static extern TestProject.tjscalingfactor* tjGetScalingFactors(int* numscalingfactors);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjInitTransform", CallingConvention = CallingConvention.Cdecl)]
	public static extern TestProject.tjhandle tjInitTransform();
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjTransform", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjTransform(TestProject.tjhandle handle,byte* jpegBuf,uint jpegSize,int n,byte** dstBufs,uint* dstSizes,TestProject.tjtransform* transforms,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjBufSizeYUV2", CallingConvention = CallingConvention.Cdecl)]
	public static extern uint tjBufSizeYUV2(int width,int align,int height,int subsamp);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjCompressFromYUV", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjCompressFromYUV(TestProject.tjhandle handle,byte* srcBuf,int width,int align,int height,int subsamp,byte** jpegBuf,uint* jpegSize,int jpegQual,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjCompressFromYUVPlanes", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjCompressFromYUVPlanes(TestProject.tjhandle handle,byte** srcPlanes,int width,int* strides,int height,int subsamp,byte** jpegBuf,uint* jpegSize,int jpegQual,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjDecodeYUV", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjDecodeYUV(TestProject.tjhandle handle,byte* srcBuf,int align,int subsamp,byte* dstBuf,int width,int pitch,int height,int pixelFormat,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjDecodeYUVPlanes", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjDecodeYUVPlanes(TestProject.tjhandle handle,byte** srcPlanes,int* strides,int subsamp,byte* dstBuf,int width,int pitch,int height,int pixelFormat,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjDecompressHeader3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjDecompressHeader3(TestProject.tjhandle handle,byte* jpegBuf,uint jpegSize,int* width,int* height,int* jpegSubsamp,int* jpegColorspace);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjDecompressToYUV2", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjDecompressToYUV2(TestProject.tjhandle handle,byte* jpegBuf,uint jpegSize,byte* dstBuf,int width,int align,int height,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjDecompressToYUVPlanes", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjDecompressToYUVPlanes(TestProject.tjhandle handle,byte* jpegBuf,uint jpegSize,byte** dstPlanes,int width,int* strides,int height,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjEncodeYUV3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjEncodeYUV3(TestProject.tjhandle handle,byte* srcBuf,int width,int pitch,int height,int pixelFormat,byte* dstBuf,int align,int subsamp,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjEncodeYUVPlanes", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjEncodeYUVPlanes(TestProject.tjhandle handle,byte* srcBuf,int width,int pitch,int height,int pixelFormat,byte** dstPlanes,int* strides,int subsamp,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjPlaneHeight", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjPlaneHeight(int componentID,int height,int subsamp);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjPlaneSizeYUV", CallingConvention = CallingConvention.Cdecl)]
	public static extern uint tjPlaneSizeYUV(int componentID,int width,int stride,int height,int subsamp);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjPlaneWidth", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjPlaneWidth(int componentID,int width,int subsamp);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjGetErrorCode", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjGetErrorCode(TestProject.tjhandle handle);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjGetErrorStr2", CallingConvention = CallingConvention.Cdecl)]
	public static extern byte* tjGetErrorStr2(TestProject.tjhandle handle);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjLoadImage", CallingConvention = CallingConvention.Cdecl)]
	public static extern byte* tjLoadImage(byte* filename,int* width,int align,int* height,int* pixelFormat,int flags);
	[System.Runtime.InteropServices.DllImportAttribute("TestProject.Interop", EntryPoint = "tjSaveImage", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tjSaveImage(byte* filename,byte* buffer,int width,int pitch,int height,int pixelFormat,int flags);
}
