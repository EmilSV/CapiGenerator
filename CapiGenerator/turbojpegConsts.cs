namespace Turbojpeg;

public static partial class TurbojpegConsts
{
	public const long TJ_NUMINIT = 3;
	public const long TJ_NUMSAMP = 7;
	public const long TJ_NUMPF = 12;
	public const long TJ_NUMCS = 5;
	public const long TJ_NUMERR = 2;
	public const long TJ_NUMXOP = 8;
	public const long TJXOPT_PERFECT = (1<<0);
	public const long TJXOPT_TRIM = (1<<1);
	public const long TJXOPT_CROP = (1<<2);
	public const long TJXOPT_GRAY = (1<<3);
	public const long TJXOPT_NOOUTPUT = (1<<4);
	public const long TJXOPT_PROGRESSIVE = (1<<5);
	public const long TJXOPT_COPYNONE = (1<<6);
	public const long TJXOPT_ARITHMETIC = (1<<7);
	public const long TJXOPT_OPTIMIZE = (1<<8);
	public const long NUMSUBOPT = TJ_NUMSAMP;
	public const long TJ_BGR = 1;
	public const long TJ_BOTTOMUP = TJFLAG_BOTTOMUP;
	public const long TJ_FORCEMMX = TJFLAG_FORCEMMX;
	public const long TJ_FORCESSE = TJFLAG_FORCESSE;
	public const long TJ_FORCESSE2 = TJFLAG_FORCESSE2;
	public const long TJ_ALPHAFIRST = 64;
	public const long TJ_FORCESSE3 = TJFLAG_FORCESSE3;
	public const long TJ_FASTUPSAMPLE = TJFLAG_FASTUPSAMPLE;
	public const long TJ_YUV = 512;
	public const long TJFLAG_BOTTOMUP = 2;
	public const long TJFLAG_FORCEMMX = 8;
	public const long TJFLAG_FORCESSE = 16;
	public const long TJFLAG_FORCESSE2 = 32;
	public const long TJFLAG_FORCESSE3 = 128;
	public const long TJFLAG_FASTUPSAMPLE = 256;
	public const long TJFLAG_NOREALLOC = 1024;
	public const long TJFLAG_FASTDCT = 2048;
	public const long TJFLAG_ACCURATEDCT = 4096;
	public const long TJFLAG_STOPONWARNING = 8192;
	public const long TJFLAG_PROGRESSIVE = 16384;
	public const long TJFLAG_LIMITSCANS = 32768;
}