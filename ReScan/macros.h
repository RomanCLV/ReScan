#ifndef RESCAN_MACROS
#define RESCAN_MACROS

namespace ReScan
{
#define ZERO_CLAMP						0.000001

#define SUCCESS_CODE					0

#define MEMORY_ALLOCATION_ERROR_CODE	1
#define FILE_NOT_FOUND_ERROR_CODE		2

#define INVALID_FILE_ERROR_CODE			100
#define INVALID_PLAN_ERROR_CODE			101

#define SAVE_CONFIG_ERROR_CODE          200
#define READ_CONFIG_ERROR_CODE          201
#define SET_CONFIG_ERROR_CODE           202

#define NO_MATRIX_INVERSE_ERROR_CODE	300
}

#endif // RESCAN_MACROS