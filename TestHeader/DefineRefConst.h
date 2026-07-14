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

#define TEST_SOME_ADD(a, b)  (a + b)

#define TEST_MAX_SINT64  TEST_SINT64_C(0x7FFFFFFFFFFFFFFF)   /* 9223372036854775807 */


#define TEST_MAX_TIME TEST_MAX_SINT64

#define TEST_ADD_VALUE TEST_SOME_ADD(1, 2)

#define TEST_IDENTITY(value) value
#define TEST_NESTED_ADD TEST_IDENTITY(TEST_SOME_ADD(1, TEST_IDENTITY(2)))

#define TEST_ZERO() 7
#define TEST_ZERO_VALUE TEST_ZERO()

#define TEST_RECURSIVE(value) TEST_RECURSIVE(value)
#define TEST_RECURSIVE_VALUE TEST_RECURSIVE(1)
#define TEST_WRONG_ARITY TEST_SOME_ADD(1)
