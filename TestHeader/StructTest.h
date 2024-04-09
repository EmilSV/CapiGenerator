#include <stdint.h>
#include <stddef.h>

typedef struct SimpleStruct
{
    char const *label;
    uint32_t baseMipLevel;
    uint32_t mipLevelCount;
    uint32_t baseArrayLayer;
    uint32_t arrayLayerCount;
} SimpleStruct;

typedef enum WGPUBufferBindingType
{
    WGPUBufferBindingType_Undefined = 0x00000000,
    WGPUBufferBindingType_Uniform = 0x00000001,
    WGPUBufferBindingType_Storage = 0x00000002,
    WGPUBufferBindingType_ReadOnlyStorage = 0x00000003,
    WGPUBufferBindingType_Force32 = 0x7FFFFFFF
} WGPUBufferBindingType;

typedef struct WGPUBufferBindingLayout
{
    WGPUBufferBindingType type;
    uint32_t hasDynamicOffset;
    uint64_t minBindingSize;
} WGPUBufferBindingLayout;