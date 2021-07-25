///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2011 EaseFilter Technologies Inc.
//    All Rights Reserved
//
//    This software is part of a licensed software product and may
//    only be used or copied in accordance with the terms of that license.
//
//	  This header file includes the structures and exported API from the FilterAPI.DLL
//	    	
//
///////////////////////////////////////////////////////////////////////////////


#ifndef __FILTER_API_H__
#define __FILTER_API_H__


#define STATUS_ACCESS_DENIED				0xC0000022L

#define MESSAGE_SEND_VERIFICATION_NUMBER	0xFF000001
#define BLOCK_SIZE							65536
#define MAX_FILE_NAME_LENGTH				1024
#define MAX_SID_LENGTH						256
#define MAX_PATH							260

//
//define the filter driver request message type
//
#define MESSAGE_TYPE_RESTORE_BLOCK_OR_FILE		0x00000001   //require to restore the block data or full data of the stub file
#define MESSAGE_TYPE_RESTORE_FILE				0x00000002	 //require to restore the full data of the stub file,for memory mapping file open or write request,we need to restore the stub file
#define MESSAGE_TYPE_RESTORE_FILE_TO_CACHE		0x00000008   //require to download the whole file to the cache folder
#define MESSAGE_TYPE_SEND_EVENT_NOTIFICATION	0x00000010	 //the send notification event request,don't need to reply this request.

#define REPARSETAG_KEY		0xbba65d6f

typedef struct _REPARSETAG_DATA 
{
	ULONG		ReparseTagKey;
	ULONG		Flags;
	ULONG		FileNameLength;
	WCHAR		FileName[1];
	
} REPARSETAG_DATA, *PREPARSETAG_DATA;

typedef enum _BooleanConfigId 
{
    ENABLE_NO_RECALL_FLAG = 0x00000001, 

} BooleanConfigId;

//this is the data structure which send data from kernel to user mode.
typedef struct _MESSAGE_SEND_DATA 
{
	ULONG			MessageId;
	PVOID			FileObject;
	PVOID			FsContext;
	ULONG			MessageType;	
	ULONG			ProcessId;
    ULONG			ThreadId;   
	LONGLONG		Offset; // read/write offset 
	ULONG			Length; //read/write length
	LONGLONG		FileSize;
	LONGLONG		TransactionTime;
	LONGLONG		CreationTime;
	LONGLONG		LastAccessTime;
	LONGLONG		LastWriteTime;
	ULONG			FileAttributes;
	//The disired access,share access and disposition for Create request.
	ULONG			DesiredAccess;
	ULONG			Disposition;
	ULONG			ShareAccess;
	ULONG			CreateOptions;
	ULONG			CreateStatus;

	//For QueryInformation,SetInformation,Directory request it is information class
	//For QuerySecurity and SetSecurity request,it is securityInformation.
	ULONG			InfoClass; 

	ULONG			Status;
	ULONG			FileNameLength;
	WCHAR			FileName[MAX_FILE_NAME_LENGTH];
	ULONG			SidLength;
    UCHAR			Sid[MAX_SID_LENGTH];
	ULONG			DataBufferLength;
	UCHAR			DataBuffer[BLOCK_SIZE];

	ULONG			VerificationNumber;

} MESSAGE_SEND_DATA, *PMESSAGE_SEND_DATA;

typedef enum _EVENT_TYPE 
{
	FILE_CREATEED = 0x00000020,
    FILE_CHANGED = 0x00000040,
    FILE_RENAMED = 0x00000080,
    FILE_DELETED = 0x00000100,
}EVENT_TYPE;

//The status return to filter,instruct filter what process needs to be done.
typedef enum _FilterStatus 
{
	FILTER_MESSAGE_IS_DIRTY			= 0x00000001, //Set this flag if the reply message need to be processed.
	FILTER_COMPLETE_PRE_OPERATION	= 0x00000002, //Set this flag if complete the pre operation. 
	FILTER_DATA_BUFFER_IS_UPDATED	= 0x00000004, //Set this flag if the databuffer was updated.
	BLOCK_DATA_WAS_RETURNED			= 0x00000008, //Set this flag if return read block databuffer to filter.
	CACHE_FILE_WAS_RETURNED			= 0x00000010, //Set this flag if the cache file was restored.
	REHYDRATE_FILE_VIA_CACHE_FILE	= 0x00000020, //Set this flag if the whole cache file was downloaded and stub file needs to be rehydrated.
	
} FilterStatus;

//This the structure return back to filter,only for call back filter.
typedef struct _MESSAGE_REPLY_DATA 
{
	ULONG		MessageId;
	ULONG		MessageType;	
	ULONG		ReturnStatus;
	ULONG		FilterStatus;
	ULONG		DataBufferLength;
	UCHAR		DataBuffer[BLOCK_SIZE];	

} MESSAGE_REPLY_DATA, *PMESSAGE_REPLY_DATA;


extern "C" __declspec(dllexport) 
BOOL 
InstallDriver();

extern "C" __declspec(dllexport) 
BOOL
UnInstallDriver();

extern "C" __declspec(dllexport) 
BOOL
SetRegistrationKey(char* key);

extern "C" __declspec(dllexport) 
BOOL 
AddFilterRule(ULONG AccessFlag, WCHAR* FilterMask, WCHAR* ReparseMask);

typedef BOOL (__stdcall *Proto_Message_Callback)(
   IN		PMESSAGE_SEND_DATA pSendMessage,
   IN OUT	PMESSAGE_REPLY_DATA pReplyMessage);

typedef VOID (__stdcall *Proto_Disconnect_Callback)();

extern "C" __declspec(dllexport) 
BOOL
RegisterMessageCallback(
	ULONG ThreadCount,
	Proto_Message_Callback MessageCallback,
	Proto_Disconnect_Callback DisconnectCallback );

extern "C" __declspec(dllexport) 
VOID
Disconnect();

extern "C" __declspec(dllexport) 
BOOL
GetLastErrorMessage(WCHAR* Buffer, PULONG BufferLength);

extern "C" __declspec(dllexport)
BOOL
ResetConfigData();

extern "C" __declspec(dllexport)  
BOOL
SetBooleanConfig(ULONG booleanConfig);

extern "C" __declspec(dllexport)  
BOOL
SetConnectionTimeout(ULONG TimeOutInSeconds);

extern "C" __declspec(dllexport) 
BOOL 
AddExcludedProcessId(ULONG ProcessId);

extern "C" __declspec(dllexport) 
BOOL 
RemoveExcludeProcessId(ULONG ProcessId);

extern "C" __declspec(dllexport) 
BOOL	
GetFileHandleInFilter(WCHAR* FileName,ULONG  DesiredAccess,HANDLE*	FileHandle);


extern "C" __declspec(dllexport) 
BOOL
IsDriverServiceRunning();

extern "C" __declspec(dllexport) 
BOOL
OpenStubFile(
    LPCTSTR fileName,
    DWORD   dwDesiredAccess,
    DWORD   dwShareMode,
    PHANDLE pHandle );

extern "C" __declspec(dllexport) 
BOOL
CreateStubFile(
	LPCTSTR		fileName,
	LONGLONG	fileSize,
	ULONG		fileAttributes,
	ULONG		tagDataLength,
	BYTE*		tagData,
	BOOL		overwriteIfExist,
	PHANDLE		pHandle );

extern "C" __declspec(dllexport) 
BOOL
GetTagData(
	HANDLE hFile,
	PULONG tagDataLength,
	BYTE*  tagData);

extern "C" __declspec(dllexport) 
BOOL  
RemoveTagData(
    HANDLE hFile );

extern "C" __declspec(dllexport) 
BOOL 
AddTagData(
    HANDLE  hFile, 
    ULONG   tagDataLength,
	BYTE*	tagData );

#endif //FILTER_API_H
