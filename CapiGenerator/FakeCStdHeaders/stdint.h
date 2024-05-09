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

#define INT_LEAST8_MIN INT8_MIN
#define INT_LEAST16_MIN INT16_MIN
#define INT_LEAST32_MIN INT32_MIN
#define INT_LEAST64_MIN INT64_MIN
#define INT_LEAST8_MAX INT8_MAX
#define INT_LEAST16_MAX INT16_MAX
#define INT_LEAST32_MAX INT32_MAX
#define INT_LEAST64_MAX INT64_MAX
#define UINT_LEAST8_MAX UINT8_MAX
#define UINT_LEAST16_MAX UINT16_MAX
#define UINT_LEAST32_MAX UINT32_MAX
#define UINT_LEAST64_MAX UINT64_MAX

#define INT_FAST8_MIN INT8_MIN
#define INT_FAST16_MIN INT32_MIN
#define INT_FAST32_MIN INT32_MIN
#define INT_FAST64_MIN INT64_MIN
#define INT_FAST8_MAX INT8_MAX
#define INT_FAST16_MAX INT32_MAX
#define INT_FAST32_MAX INT32_MAX
#define INT_FAST64_MAX INT64_MAX
#define UINT_FAST8_MAX UINT8_MAX
#define UINT_FAST16_MAX UINT32_MAX
#define UINT_FAST32_MAX UINT32_MAX
#define UINT_FAST64_MAX UINT64_MAX
