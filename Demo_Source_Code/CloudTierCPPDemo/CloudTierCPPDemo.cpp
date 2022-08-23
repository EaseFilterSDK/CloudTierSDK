///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2011 EaseFilter Technologies.
//    All Rights Reserved
//
//    This software is part of a licensed software product and may
//    only be used or copied in accordance with the terms of that license.
//
///////////////////////////////////////////////////////////////////////////////

// CPlusPlusDemo.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "Tools.h"
#include "UnitTest.h"

#define	MAX_ERROR_MESSAGE_SIZE	1024

#define PrintMessage	wprintf //ToDebugger

//set the include directory for this libary to the right folder, for this demo, the library is in folder Bin\x64  or Bin\Win32
#pragma comment(lib,"FilterAPI.lib")

VOID
__stdcall
DisconnectCallback();

BOOL
__stdcall
MessageCallback(
   IN		PMESSAGE_SEND_DATA pSendMessage,
   IN OUT	PMESSAGE_REPLY_DATA pReplyMessage);

void 
SendConfigInfoToFilter(ULONG FilterType,WCHAR* FilterFolder,ULONG IoRegistration ,ULONG AccessFlag);


VOID
Usage (
    VOID
    )
/*++

Routine Description

    Prints usage

Arguments

    None

Return Value

    None

--*/
{

	printf( "\nUsage:		CPlusPlusExample  <option>\n" );
	printf( "\noption:\n" );
	printf( "		i ----- Install Driver\n" );
	printf( "		u ----- UnInstall Driver\n" );
	printf( "		t ----- Driver UnitTest, and start the stub file service.\n" );
	
}


int _tmain(int argc, _TCHAR* argv[])
{
    DWORD	threadCount = 5;
	BOOL	ret = FALSE;
	
	//Purchase a license key with the link: http://www.EaseFilter.com/Order.htm
    //Email us to request a trial key: info@EaseFilter.com //free email is not accepted.

	char*	registerKey = "***************************************************";

	if(argc <= 1)
	{
		Usage();
		return 1;
	}

	TCHAR op = *argv[1];

	switch(op)
	{
		case 'i': //install driver
		{
			//Install the driver only once.
			//After the installation, you can use command "fltmc unload CloudTier" to unload the driver
			//or "fltmc load CloudTier" to load the driver, or "fltmc" to check the status of the driver.
			ret = InstallDriver();	
			if( !ret )
			{
				PrintLastErrorMessage( L"InstallDriver failed.",__LINE__);
				return 1;
			}

			PrintPassedMessage(L"Install filter driver succeeded!");

			break;
		}

		case 'u': //uninstall driver
		{
			ret = UnInstallDriver();
			if( !ret )
			{
				PrintLastErrorMessage( L"UnInstallDriver failed.",__LINE__);
				return 1;
			}

			PrintPassedMessage(L"UnInstall filter driver succeeded!");

			break;

		}

		case 't': //driver unit test 
		{

			ret = UnInstallDriver();
			Sleep(2000);

			ret = InstallDriver();	
			if( !ret )
			{
				PrintLastErrorMessage( L"InstallDriver failed.",__LINE__);
				return 1;
			}

			PrintPassedMessage(L"Install filter driver succeeded!");


			ret = SetRegistrationKey(registerKey);
			if( !ret )
			{
				PrintLastErrorMessage( L"SetRegistrationKey failed.",__LINE__);
				return 1;
			}

			ret = RegisterMessageCallback(threadCount,MessageCallback,DisconnectCallback);
			if( !ret )
			{
				PrintLastErrorMessage( L"RegisterMessageCallback failed.",__LINE__);
				return 1;
			}			

			//this the demo how to use the stub file API.
			FilterDriverUnitTest();

			printf("\r\n\r\n\r\nTo do more test, you can copy your test files to folder c:\\CloudTierUnitTest\\sourceFileFolder,\r\nthe same stub file associated to the test file will be created in folder c:\\CloudTierUnitTest\\testStubFileFolder,\r\nyou can open the stub file in testStubFileFolder.\r\n\r\n\r\n."); 

			system("pause");

			Disconnect();

			break;

		}

		default:
			{
				Usage(); 
				return 1;
			}

	}
		
	return 0;
}


BOOL
DownloadCacheFile(  
	IN		PMESSAGE_SEND_DATA pSendMessage,
	IN OUT	PMESSAGE_REPLY_DATA pReplyMessage,
	IN		BOOL rehydrateStubFile)
{
	//for our test, we only assume that if the file exist in the sourceFile folder, then this is our test files,
	//we will process it, or we return false;
	WCHAR* sourceFileName = (PWCHAR)pSendMessage->DataBuffer;

	printf("Get the source file name %ws from the tag data.\n",sourceFileName);

	if(GetFileAttributes(sourceFileName) != INVALID_FILE_ATTRIBUTES)
	{
		if(rehydrateStubFile)
		{
			//if you want to rehydrate the file please return this flag
			pReplyMessage->FilterStatus = REHYDRATE_FILE_VIA_CACHE_FILE;
		}
		else
		{
			pReplyMessage->FilterStatus = CACHE_FILE_WAS_RETURNED;
		}

		pReplyMessage->ReturnStatus = STATUS_SUCCESS;
		
		ULONG dataLength = (ULONG)wcslen(sourceFileName)*2;

		//set your actual return data length here.
		pReplyMessage->DataBufferLength = dataLength;
		memcpy(pReplyMessage->DataBuffer,sourceFileName,dataLength);

		printf("\nReturn whole cache file %ws FilterStatus:%0x to the filter driver\n",sourceFileName,pReplyMessage->FilterStatus);

		return TRUE;
	}
	else
	{
		PrintErrorMessage( L"Get the source file failed.\n\n",0); 
		return FALSE;
	}
}

BOOL
__stdcall
MessageCallback(
   IN		PMESSAGE_SEND_DATA pSendMessage,
   IN OUT	PMESSAGE_REPLY_DATA pReplyMessage)
{
	BOOL	ret = TRUE;

	WCHAR userName[MAX_PATH];
	WCHAR domainName[MAX_PATH];

	int userNameSize = MAX_PATH;
	int domainNameSize = MAX_PATH;
	SID_NAME_USE snu;

	__try
	{
		BOOL ret = LookupAccountSid( NULL,
									pSendMessage->Sid,
									userName,
									(LPDWORD)&userNameSize,
									domainName,
									(LPDWORD)&domainNameSize,
									&snu); 
	
		PrintMessage( L"\nId# %d MessageType:0X%0x UserName:%ws\\%ws\nProcessId:%d ThreadId:%d Return Status:%0x\nFileSize:%I64d Attributes:%0x FileName:%ws\n"
			,pSendMessage->MessageId,pSendMessage->MessageType,domainName,userName
			,pSendMessage->ProcessId,pSendMessage->ThreadId,pSendMessage->Status
			,pSendMessage->FileSize,pSendMessage->FileAttributes,pSendMessage->FileName);

		if(	MESSAGE_TYPE_SEND_EVENT_NOTIFICATION == pSendMessage->MessageType	)
		{
			ULONG eventType = pSendMessage->InfoClass;

			switch(eventType)
			{
				case FILE_CREATEED: PrintMessage(L"FILE_CREATEED Event,new file %ws was created.\n",pSendMessage->FileName);break;
				case FILE_CHANGED: PrintMessage(L"FILE_CHANGED Event,file %ws was modified.\n",pSendMessage->FileName);break;
				case FILE_RENAMED: PrintMessage(L"FILE_RENAMED Event,file %ws was rename to %ws.\n",pSendMessage->FileName,pSendMessage->DataBuffer);break;
				case FILE_DELETED: PrintMessage(L"FILE_DELETED Event,file %ws was deleted.\n",pSendMessage->FileName);break;
				default:PrintMessage(L"Unknow eventType:%0x.\n",eventType);break;
			}

			goto EXIT;
		}

		printf("\nTag data length:%d   \nData:%s\n\n",pSendMessage->DataBufferLength,pSendMessage->DataBuffer);

		//There are two types of stub file read from the filter driver:
		//1. MESSAGE_TYPE_RESTORE_FILE_TO_CACHE:
		//When the stub file was written in first time, the filter driver needs to restore the whole file first,you need to create 
		//a cache file and return to the filter driver,the filter driver will rehydrate the stub file with the cache file data.

		//For the memory mapped file open( for example open file with notepad in local computer),it requires to download the whole 
		//cache file and return it to filter driver, the filter driver will read the cache file data.

		//2. MESSAGE_TYPE_RESTORE_BLOCK_OR_FILE: 
		//For this stub file read request type, you can return the requested block data to filter driver, or you can return
		//the whole cache file to filter driver, if you return whole cache file, the filter driver will read data from the 
		//cache file, if you return the whole cache. 
		//
		//Return filter status, there are three return status you can return:
		//1. BLOCK_DATA_WAS_RETURNED: return the rquested block data to the filter driver, if you want to read the sepecific blocks of the data.
		//2. CACHE_FILE_WAS_RETURNED: return the cache file to the filter driver if the soure data was downloaded completely to a cache file.
		//3. REHYDRATE_FILE_VIA_CACHE_FILE: the stub file will be rehydrated if the soure data was downloaded completely to a cache file.

		if(	MESSAGE_TYPE_RESTORE_FILE_TO_CACHE == pSendMessage->MessageType	)
		{
			ret = DownloadCacheFile(pSendMessage,pReplyMessage,false);
			if(!ret)
			{
				
				PrintLastErrorMessage( L"DownloadCacheFile failed.",__LINE__);

				pReplyMessage->ReturnStatus = STATUS_UNSUCCESSFUL;
			}
		}
		else if ( MESSAGE_TYPE_RESTORE_BLOCK_OR_FILE == pSendMessage->MessageType )
		{
			if( IsRehydrateTestFile(pSendMessage->FileName))
			{
				ret = DownloadCacheFile(pSendMessage,pReplyMessage,true);
				if(!ret)
				{					
					PrintLastErrorMessage( L"DownloadCacheFile failed.",__LINE__);
					pReplyMessage->ReturnStatus = STATUS_UNSUCCESSFUL;
				}
			}
			else if( IsRestoreCacheTestFile(pSendMessage->FileName))
			{
				ret = DownloadCacheFile(pSendMessage,pReplyMessage,false);
				if(!ret)
				{					
					PrintLastErrorMessage( L"DownloadCacheFile failed.",__LINE__);
					pReplyMessage->ReturnStatus = STATUS_UNSUCCESSFUL;
				}
			}
			else if ( IsBlockTestFile(pSendMessage->FileName))
			{
				LONGLONG readOffset = pSendMessage->Offset;
				ULONG	readLength = pSendMessage->Length;
				LONGLONG fileSize = pSendMessage->FileSize;

				//do your own verififcation here.

				if(		readOffset >= fileSize 
					||	readLength > BLOCK_SIZE)
					
				{
					pReplyMessage->ReturnStatus = STATUS_END_OF_FILE;
					goto EXIT;
				}				
			
				//here you can get your data from remote server

				CHAR* testData = GetTestData();
				ULONG dataLength = (ULONG)strlen(testData);

				//make sure the return data length can't exceed the BLOCK_SIZE
				if( dataLength > BLOCK_SIZE )
				{
					dataLength = BLOCK_SIZE;
				}

				//set your actual return data length here.
				pReplyMessage->DataBufferLength = dataLength;
				memcpy(pReplyMessage->DataBuffer,testData,dataLength);

				pReplyMessage->FilterStatus = BLOCK_DATA_WAS_RETURNED;
				pReplyMessage->ReturnStatus = STATUS_SUCCESS;

				printf("\nStubFile return block data succeeded.Offset:%I64d Length:%d returnLength:%d\n\n"
					,readOffset,readLength,dataLength);

			}
			else
			{
				ret = DownloadCacheFile(pSendMessage,pReplyMessage,false);
				if(!ret)
				{					
					PrintLastErrorMessage( L"DownloadCacheFile failed.",__LINE__);
					pReplyMessage->ReturnStatus = STATUS_UNSUCCESSFUL;
				}
			}
					
		}

	}
	__except( EXCEPTION_EXECUTE_HANDLER  )
	{
		PrintErrorMessage( L"MessageCallback failed.",GetLastError());     
	}

EXIT:

	return ret;
}

VOID
__stdcall
DisconnectCallback()
{
	PrintLastErrorMessage(L"Filter connection was disconnected.",__LINE__);
	return;
}



