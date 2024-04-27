/**
 * Cropping region
 */
typedef struct {
  /**
   * The left boundary of the cropping region.  This must be evenly divisible
   * by the MCU block width (see #tjMCUWidth.)
   */
  int x;
  /**
   * The upper boundary of the cropping region.  For lossless transformation,
   * this must be evenly divisible by the MCU block height (see #tjMCUHeight.)
   */
  int y;
  /**
   * The width of the cropping region.  Setting this to 0 is the equivalent of
   * setting it to the width of the source JPEG image - x.
   */
  int w;
  /**
   * The height of the cropping region.  Setting this to 0 is the equivalent of
   * setting it to the height of the source JPEG image - y.
   */
  int h;
} tjregion;

/**
 * Lossless transform
 */
typedef struct tjtransform {
  /**
   * Cropping region
   */
  tjregion r;
  /**
   * One of the @ref TJXOP "transform operations"
   */
  int op;
  /**
   * The bitwise OR of one of more of the @ref TJXOPT_ARITHMETIC
   * "transform options"
   */
  int options;
  /**
   * Arbitrary data that can be accessed within the body of the callback
   * function
   */
  void *data;
  /**
   * A callback function that can be used to modify the DCT coefficients after
   * they are losslessly transformed but before they are transcoded to a new
   * JPEG image.  This allows for custom filters or other transformations to be
   * applied in the frequency domain.
   *
   * @param coeffs pointer to an array of transformed DCT coefficients.  (NOTE:
   * this pointer is not guaranteed to be valid once the callback returns, so
   * applications wishing to hand off the DCT coefficients to another function
   * or library should make a copy of them within the body of the callback.)
   *
   * @param arrayRegion #tjregion structure containing the width and height of
   * the array pointed to by `coeffs` as well as its offset relative to the
   * component plane.  TurboJPEG implementations may choose to split each
   * component plane into multiple DCT coefficient arrays and call the callback
   * function once for each array.
   *
   * @param planeRegion #tjregion structure containing the width and height of
   * the component plane to which `coeffs` belongs
   *
   * @param componentID ID number of the component plane to which `coeffs`
   * belongs.  (Y, Cb, and Cr have, respectively, ID's of 0, 1, and 2 in
   * typical JPEG images.)
   *
   * @param transformID ID number of the transformed image to which `coeffs`
   * belongs.  This is the same as the index of the transform in the
   * `transforms` array that was passed to #tj3Transform().
   *
   * @param transform a pointer to a #tjtransform structure that specifies the
   * parameters and/or cropping region for this transform
   *
   * @return 0 if the callback was successful, or -1 if an error occurred.
   */
  int (*customFilter) (short *coeffs, tjregion arrayRegion,
                       tjregion planeRegion, int componentID, int transformID,
                       struct tjtransform *transform);
} tjtransform;