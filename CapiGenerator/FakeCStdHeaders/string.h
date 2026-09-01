#ifndef CAPI_GENERATOR_FAKE_C_STD_HEADERS_STRING_H
#define CAPI_GENERATOR_FAKE_C_STD_HEADERS_STRING_H

#include <stddef.h>

void *memcpy(void *destination, const void *source, size_t count);
void *memmove(void *destination, const void *source, size_t count);
void *memset(void *destination, int value, size_t count);

#endif // CAPI_GENERATOR_FAKE_C_STD_HEADERS_STRING_H
