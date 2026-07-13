#include <stdint.h>

#if defined(INT64_C)
#define TEST_SINT64_C(c)  INT64_C(c)
#elif defined(_MSC_VER)
#define TEST_SINT64_C(c)  c ## i64
#elif defined(__LP64__) || defined(_LP64)
#define TEST_SINT64_C(c)  c ## L
#else
#define TEST_SINT64_C(c)  c ## LL
#endif

#define TEST_MAX_SINT64  TEST_SINT64_C(0x7FFFFFFFFFFFFFFF)   /* 9223372036854775807 */


#define TEST_MAX_TIME TEST_MAX_SINT64
