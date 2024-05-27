#ifndef CAPI_GENERATOR_FAKE_C_STD_HEADERS_STDINT_H

#define CAPI_GENERATOR_FAKE_C_STD_HEADERS_STDINT_H

typedef signed char int8_t;
typedef short int16_t;
typedef int int32_t;
typedef long long int64_t;
typedef unsigned char uint8_t;
typedef unsigned short uint16_t;
typedef unsigned int uint32_t;
typedef unsigned long long uint64_t;

typedef signed char int_least8_t;
typedef short int_least16_t;
typedef int int_least32_t;
typedef long long int_least64_t;
typedef unsigned char uint_least8_t;
typedef unsigned short uint_least16_t;
typedef unsigned int uint_least32_t;
typedef unsigned long long uint_least64_t;

typedef signed char int_fast8_t;
typedef int int_fast16_t;
typedef int int_fast32_t;
typedef long long int_fast64_t;
typedef unsigned char uint_fast8_t;
typedef unsigned int uint_fast16_t;
typedef unsigned int uint_fast32_t;
typedef unsigned long long uint_fast64_t;

typedef long long intmax_t;
typedef unsigned long long uintmax_t;

typedef long long intptr_t;
typedef unsigned long long uintptr_t;

#define SIZE_MAX __$SIZE_MAX$__
#define INT8_MIN __$INT8_MIN$__
#define INT16_MIN __$INT16_MIN$__
#define INT32_MIN __$INT32_MIN$__
#define INT64_MIN __$INT64_MIN$__
#define INT8_MAX __$INT8_MAX$__
#define INT16_MAX __$INT16_MAX$__
#define INT32_MAX __$INT32_MAX$__
#define INT64_MAX __$INT64_MAX$__
#define UINT8_MAX __$UINT8_MAX$__
#define UINT16_MAX __$UINT16_MAX$__
#define UINT32_MAX __$UINT32_MAX$__
#define UINT64_MAX __$UINT64_MAX$__

#define INTPTR_MIN __$INTPTR_MIN$__
#define INTPTR_MAX __$INTPTR_MAX$__
#define UINTPTR_MAX __$UINTPTR_MAX$__

#define INT8_C(x) (x)
#define INT16_C(x) (x)
#define INT32_C(x) (x)
#define INT64_C(x) (x##LL)

#define UINT8_C(x) (x)
#define UINT16_C(x) (x)
#define UINT32_C(x) (x##U)
#define UINT64_C(x) (x##ULL)

#define INTMAX_C(x) INT64_C(x)
#define UINTMAX_C(x) UINT64_C(x)

#define INT_LEAST8_MIN __$INT8_MIN$__
#define INT_LEAST16_MIN __$INT16_MIN$__
#define INT_LEAST32_MIN __$INT32_MIN$__
#define INT_LEAST64_MIN __$INT64_MIN$__
#define INT_LEAST8_MAX __$INT8_MAX$__
#define INT_LEAST16_MAX __$INT16_MAX$__
#define INT_LEAST32_MAX __$INT32_MAX$__
#define INT_LEAST64_MAX __$INT64_MAX$__
#define UINT_LEAST8_MAX __$UINT8_MAX$__
#define UINT_LEAST16_MAX __$UINT16_MAX$__
#define UINT_LEAST32_MAX __$UINT32_MAX$__
#define UINT_LEAST64_MAX __$UINT64_MAX$__

#define INT_FAST8_MIN __$INT8_MIN$__
#define INT_FAST16_MIN __$INT16_MIN$__
#define INT_FAST32_MIN __$INT32_MIN$__
#define INT_FAST64_MIN __$INT64_MIN$__
#define INT_FAST8_MAX __$INT8_MAX$__
#define INT_FAST16_MAX __$INT16_MAX$__
#define INT_FAST32_MAX __$INT32_MAX$__
#define INT_FAST64_MAX __$INT64_MAX$__
#define UINT_FAST8_MAX __$UINT8_MAX$__
#define UINT_FAST16_MAX __$UINT16_MAX$__
#define UINT_FAST32_MAX __$UINT32_MAX$__
#define UINT_FAST64_MAX __$UINT64_MAX$__

#endif // CAPI_GENERATOR_FAKE_C_STD_HEADERS_STDINT_H