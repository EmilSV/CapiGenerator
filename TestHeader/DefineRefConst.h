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

#define TEST_UINT64_C(value) UINT64_C(value)
#define TEST_MAX_UINT64 TEST_UINT64_C(0xFFFFFFFFFFFFFFFF)

typedef int8_t TestSint8;
#define TEST_MAX_SINT8 ((TestSint8)0x7F)
#define TEST_FORMAT_ANNOTATION _Printf_format_string_
#define TEST_ANNOTATION(value)
#define TEST_EMPTY_ANNOTATION TEST_ANNOTATION(value)
#define TEST_CALL_CONVENTION __cdecl
#define TEST_ATTRIBUTE __attribute__((deprecated))
#define TEST_PLATFORM_PREDICATE (UNKNOWN_PLATFORM == 1)
#define TEST_ANNOTATION_WRAPPER(value) __attribute__((value))
#define TEST_WRAPPED_ANNOTATION TEST_ANNOTATION_WRAPPER(scoped_lockable)

#define TEST_ADD_VALUE TEST_SOME_ADD(1, 2)

#define TEST_IDENTITY(value) value
#define TEST_NESTED_ADD TEST_IDENTITY(TEST_SOME_ADD(1, TEST_IDENTITY(2)))

#define TEST_ZERO() 7
#define TEST_ZERO_VALUE TEST_ZERO()

#define TEST_RECURSIVE(value) TEST_RECURSIVE(value)
#define TEST_RECURSIVE_VALUE TEST_RECURSIVE(1)
#define TEST_WRONG_ARITY TEST_SOME_ADD(1)
