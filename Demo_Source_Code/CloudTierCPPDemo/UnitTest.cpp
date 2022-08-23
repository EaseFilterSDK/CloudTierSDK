///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2011 EaseFilter Technologies Inc.
//    All Rights Reserved
//
//    This software is part of a licensed software product and may
//    only be used or copied in accordance with the terms of that license.
//
///////////////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "Tools.h"
#include "UnitTest.h"

using namespace std;

#define	MAX_ERROR_MESSAGE_SIZE	1024

static WCHAR* testFolder = L"c:\\CloudTierUnitTest";
static CHAR* testEventFile = "c:\\CloudTierUnitTest\\eventTest\\test.txt";
static CHAR* testEventRenameFile = "c:\\CloudTierUnitTest\\eventTest\\rename.txt";
static WCHAR* testEventFolder = L"c:\\CloudTierUnitTest\\eventTest";

//the folder to store the test source file in local.
static WCHAR* sourceFileFolder = L"c:\\CloudTierUnitTest\\sourceFileFolder";
//the test soure file 
static WCHAR* unitTestSourceFile = L"c:\\CloudTierUnitTest\\sourceFileFolder\\unitTestSourceFile.txt";
static WCHAR* sourceFileFolderFilterMask = L"c:\\CloudTierUnitTest\\sourceFileFolder\\*";
//the folder to store the test stub file
static WCHAR* stubFileFolder = L"c:\\CloudTierUnitTest\\stubFileFolder";
//the stub file for the unit test
static WCHAR* unitTestStubFile = L"c:\\CloudTierUnitTest\\stubFileFolder\\unitTestStubFile.txt";
//test the stub file read return with block data 
static WCHAR* blockTestStubFile = L"c:\\CloudTierUnitTest\\stubFileFolder\\blockTestStubFile.txt";
//test the stub file read return with whole cache file
static WCHAR* wholeFileTestStubFile = L"c:\\CloudTierUnitTest\\stubFileFolder\\wholeFileTestStubFile.txt";
//test the stub file read with rehydration
static WCHAR* rehydrateStubFile = L"c:\\CloudTierUnitTest\\stubFileFolder\\rehydrateStubFile.txt";
//test the stub file open and reparse the file open to the new target file.
static WCHAR* reparseStubFile = L"c:\\CloudTierUnitTest\\stubFileFolder\\reparseStubFile.txt";
static WCHAR* reparseTargetTestFile = L"c:\\CloudTierUnitTest\\sourceFileFolder\\reparseTargetTestFile.txt"; 
//if you want to reparse to UNC file path, you can use this format: L"\\\\localhost\\c$\\CloudTierUnitTest\\targetTest.txt"

//the test file data
static CHAR* testData = "CloudTier test data which when you read the emty stub file, this data will be restored to the file as the file actual content,and the file system will read this data and return it to the user application";
static CHAR* testTagData = "This is reparse point tag data for test in the stub file. The maximum length is 16*1024 bytes.";
static ULONG testTagDataLength =  (ULONG)strlen(testTagData);

BOOL
RegisterEvent(ULONG EventType,WCHAR* FilterMask)
{
	return AddFilterRule(EventType,FilterMask,L"");
}

CHAR*
GetTestData()
{
	return testData;
}

WCHAR*
GetCacheFile()
{
	return unitTestSourceFile;
}

WCHAR*
GetCacheFolder()
{
	return sourceFileFolder;
}

BOOL
IsRehydrateTestFile(WCHAR* fileName )
{
	if( _wcsnicmp(rehydrateStubFile,fileName,wcslen(rehydrateStubFile)) == 0)
	{
		return TRUE;
	}
	else
	{
		return FALSE;
	}
}

BOOL
IsRestoreCacheTestFile(WCHAR* fileName )
{
	if( _wcsnicmp(wholeFileTestStubFile,fileName,wcslen(wholeFileTestStubFile)) == 0)
	{
		return TRUE;
	}
	else
	{
		return FALSE;
	}
}

BOOL
IsBlockTestFile(WCHAR* fileName )
{
	if( _wcsnicmp(blockTestStubFile,fileName,wcslen(blockTestStubFile)) == 0)
	{
		return TRUE;
	}
	else
	{
		return FALSE;
	}
}

//
//Create the test source file in local, when you read the stub file, we will get the data
//from the test source file and return back to the stub file.
//
VOID
CreateTestSourceFile()
{
	char* testFileContent = "This is test content for stub file.\r\n";
	WCHAR testFileName[260];

	if(!CreateDirectory(testFolder,NULL))
	{
		DWORD lastError = GetLastError();
		if( lastError != ERROR_ALREADY_EXISTS )
		{
			PrintErrorMessage(L"Create testFolder failed.", lastError);
			return;
		}
	}	

	if(!CreateDirectory(sourceFileFolder,NULL))
	{
		DWORD lastError = GetLastError();
		if( lastError != ERROR_ALREADY_EXISTS )
		{
			PrintErrorMessage(L"Create sourceFileFolder failed.", lastError);
			return;
		}
	}
	
	HANDLE hFile = CreateFile(
						unitTestSourceFile, 
						GENERIC_READ | GENERIC_WRITE,
						FILE_SHARE_READ|FILE_SHARE_WRITE,
						NULL,
						OPEN_ALWAYS,
						FILE_FLAG_OPEN_REPARSE_POINT,
						NULL);

	if( hFile == INVALID_HANDLE_VALUE )
	{
		DWORD lastError = GetLastError();
		if( lastError != ERROR_ALREADY_EXISTS )
		{
			PrintErrorMessage(L"Create unitTestSourceFile failed.", lastError);
			return;
		}		
	}


	ULONG bytesWritten = 0;
	WriteFile(hFile,testData,(DWORD)strlen(testData),&bytesWritten,NULL);

	if( INVALID_HANDLE_VALUE != hFile )
	{
	   CloseHandle(hFile);
	}
	
	hFile = CreateFile(
						reparseTargetTestFile, 
						GENERIC_READ | GENERIC_WRITE,
						FILE_SHARE_READ|FILE_SHARE_WRITE,
						NULL,
						OPEN_ALWAYS,
						FILE_FLAG_OPEN_REPARSE_POINT,
						NULL);

	if( hFile == INVALID_HANDLE_VALUE )
	{
		DWORD lastError = GetLastError();
		if( lastError != ERROR_ALREADY_EXISTS )
		{
			PrintErrorMessage(L"Create unitTestSourceFile failed.", lastError);
			return;
		}		
	}


	bytesWritten = 0;
	WriteFile(hFile,testData,(DWORD)strlen(testData),&bytesWritten,NULL);

	if( INVALID_HANDLE_VALUE != hFile )
	{
	   CloseHandle(hFile);
	}

	//create 10 test source file here.
	ZeroMemory(testFileName, sizeof(testFileName));
	for ( ULONG i = 0; i < 10; i++)
	{
		swprintf_s(testFileName,L"%s\\test.%d.txt",sourceFileFolder,i);

		HANDLE handle = CreateFile(testFileName, GENERIC_WRITE,NULL,NULL,CREATE_ALWAYS,FILE_ATTRIBUTE_NORMAL,NULL );

		if( handle == INVALID_HANDLE_VALUE)
		{
			wprintf(L"Create test file %s failed, last error is:%d",testFileName,GetLastError());
			continue;
		}

		DWORD bytesWritten = 0;

		for( ULONG j = 0; j < (i+1)*1024; j++)
		{
			if(! WriteFile(handle,(LPVOID)testFileContent,(DWORD)strlen(testFileContent),&bytesWritten,NULL))
			{
				wprintf(L"WriteFile %s failed, last error is:%d",testFileName,GetLastError());
				
				CloseHandle(handle);
				continue;
			}
		}

		CloseHandle(handle);

		wprintf(L"Created test source file %s\r\n",testFileName);

	}

}

BOOL
CreateReparseStubFile(WCHAR* StubFileName,WCHAR* ReparseFileName)
{
	BOOL ret = FALSE;	
	ULONG	reparseTagDataLength = 0;
	PREPARSETAG_DATA reparseTagData = NULL;
	HANDLE pFile = INVALID_HANDLE_VALUE;

	reparseTagDataLength =(ULONG)(sizeof(REPARSETAG_DATA) + wcslen(ReparseFileName)*sizeof(WCHAR));
	reparseTagData =(PREPARSETAG_DATA)malloc(reparseTagDataLength);

	if( NULL == reparseTagData)
	{
		PrintErrorMessage(L"No enough memory.", 0);
		ret = FALSE;
		goto EXIT;
	}

	ZeroMemory(reparseTagData,reparseTagDataLength);
	reparseTagData->ReparseTagKey = REPARSETAG_KEY;
	reparseTagData->FileNameLength = (ULONG)wcslen(ReparseFileName)*sizeof(WCHAR);
	memcpy(reparseTagData->FileName,ReparseFileName,wcslen(ReparseFileName)*sizeof(WCHAR));

	ret = CreateStubFileEx(StubFileName,strlen(testData),FILE_ATTRIBUTE_NORMAL|FILE_ATTRIBUTE_OFFLINE,reparseTagDataLength,(BYTE*)reparseTagData,0,0,0,TRUE,&pFile);
	if( !ret )
	{
		PrintLastErrorMessage( L"CreateStubFile failed.",__LINE__);
	}
EXIT:
	
	CloseHandle(pFile);

	return ret;
}

//
//To test the stub file in local, we created some source files in a local test folder, you can put your source files to 
//your own remote server or the public cloud. We will create a test stub file for every test source file, we will add the
//link to the source file as the tag data of the stub file.
//
VOID
CreateTestStubFiles()
{
	HANDLE hFile = INVALID_HANDLE_VALUE;
	LONGLONG fileSize = strlen(testData);
	ULONG fileAttribute = FILE_ATTRIBUTE_OFFLINE;
	ULONG tagDataLength = (ULONG)wcslen(unitTestSourceFile)*2;
	BYTE* tagData = (BYTE*)unitTestSourceFile;

	WIN32_FIND_DATA ffd;
	HANDLE pFile = FindFirstFile(sourceFileFolderFilterMask, &ffd);
	ULONG stubFileFolderLength = (int)wcslen(stubFileFolder)*2;

	if(!CreateDirectory(stubFileFolder,NULL))
	{
		DWORD lastError = GetLastError();
		if( lastError != ERROR_ALREADY_EXISTS )
		{
			PrintErrorMessage(L"Create stubFileFolder failed.", lastError);
			return;
		}
	}

	if(!CreateStubFileEx(unitTestStubFile,fileSize,fileAttribute|FILE_ATTRIBUTE_OFFLINE,testTagDataLength,(BYTE*)testTagData,0,0,0,TRUE,&hFile))
	{
		PrintLastErrorMessage( L"CreateStubFile failed.",__LINE__);
	}

	if( INVALID_HANDLE_VALUE != hFile )
	{
	   CloseHandle(hFile);
	}

	if(!CreateStubFileEx(blockTestStubFile,fileSize,fileAttribute|FILE_ATTRIBUTE_OFFLINE,tagDataLength,(BYTE*)tagData,0,0,0,TRUE,&hFile))
	{
		PrintLastErrorMessage( L"CreateStubFile failed.",__LINE__);
	}

	if( INVALID_HANDLE_VALUE != hFile )
	{
	   CloseHandle(hFile);
	}

	if(!CreateStubFileEx(wholeFileTestStubFile,fileSize,fileAttribute|FILE_ATTRIBUTE_OFFLINE,tagDataLength,(BYTE*)tagData,0,0,0,TRUE,&hFile))
	{
		PrintLastErrorMessage( L"CreateStubFile wholeFileTestStubFile failed.",__LINE__);
	}

	if( INVALID_HANDLE_VALUE != hFile )
	{
	   CloseHandle(hFile);
	}

	if(!CreateStubFileEx(rehydrateStubFile,fileSize,fileAttribute|FILE_ATTRIBUTE_OFFLINE,tagDataLength,(BYTE*)tagData,0,0,0,TRUE,&hFile))
	{
		PrintLastErrorMessage( L"CreateStubFile rehydrateStubFile failed.",__LINE__);
	}

	if( INVALID_HANDLE_VALUE != hFile )
	{
	   CloseHandle(hFile);
	}

	if(!CreateReparseStubFile(reparseStubFile,reparseTargetTestFile))
	{
		PrintLastErrorMessage( L"CreateReparseStubFile failed.",__LINE__);
	}


	do
    {
      if (ffd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
      {
         //this is directory, skip it.
	     continue;
      }
      else
      {
		 BOOL ret = FALSE;
		 LARGE_INTEGER fileSize;
		 HANDLE hFile = INVALID_HANDLE_VALUE;
         fileSize.LowPart = ffd.nFileSizeLow;
         fileSize.HighPart = ffd.nFileSizeHigh;

		 //the new test stub file name should be with this format:
		 // L"c:\\CloudTierUnitTest\\stubFileFolder"  + "\\" + fileName + "\0"

		 ULONG fileNameLength = (ULONG)(stubFileFolderLength + 2*sizeof(WCHAR) + 2*wcslen(ffd.cFileName));
		 WCHAR* testStubFile =  (PWCHAR)LocalAlloc(LPTR, fileNameLength);
        if (!testStubFile)
        {
			PrintLastErrorMessage( L"Allocate memory failed.",__LINE__);
			goto EXIT;
        }

		memset(testStubFile,0,fileNameLength);
		memcpy(testStubFile,stubFileFolder,stubFileFolderLength);
		testStubFile[stubFileFolderLength/2] = (WCHAR)'\\';
		memcpy((PUCHAR)testStubFile + stubFileFolderLength + 2,ffd.cFileName,(int)2*wcslen(ffd.cFileName));

		//here we set tag data with the associcated source file,in the filter callback function, you can read the data from the source file name for this stub file.
		//you can set your own tag data.
		ULONG sourceFileNameLength = (ULONG)(wcslen(sourceFileFolder)*2 + 2*sizeof(WCHAR) + 2*wcslen(ffd.cFileName));
		WCHAR* sourceFileName =  (PWCHAR)LocalAlloc(LPTR, sourceFileNameLength);
		memset(sourceFileName,0,sourceFileNameLength);
		memcpy(sourceFileName,sourceFileFolder,2*wcslen(sourceFileFolder));
		sourceFileName[wcslen(sourceFileFolder)] = (WCHAR)'\\';
		memcpy((PUCHAR)sourceFileName + 2*wcslen(sourceFileFolder) + 2,ffd.cFileName,(int)2*wcslen(ffd.cFileName));

		ret = CreateStubFileEx(testStubFile,fileSize.QuadPart,ffd.dwFileAttributes|FILE_ATTRIBUTE_OFFLINE,sourceFileNameLength,(BYTE*)sourceFileName,0,0,0,TRUE,&hFile);

		wprintf(L"Create test stub file %ws return %d.\n",testStubFile,ret);

		LocalFree(sourceFileName);
		LocalFree(testStubFile);

		if( !ret )
		{
			PrintLastErrorMessage( L"CreateStubFile failed.",__LINE__);
			goto EXIT;
		}

		if( INVALID_HANDLE_VALUE != hFile )
		{
		   CloseHandle(hFile);
		}

      }
   }
   while (FindNextFile(pFile, &ffd) != 0);

EXIT:

	if( INVALID_HANDLE_VALUE != pFile )
	{
		FindClose(pFile);
	}

}


BOOL
OpenStubFileTest()
{
    BOOL ret = FALSE;
	HANDLE hFile = INVALID_HANDLE_VALUE;

	ret = OpenStubFile(unitTestStubFile,GENERIC_READ,SHARE_READ,&hFile);
	if( !ret )
	{
		PrintLastErrorMessage( L"OpenStubFile failed.",__LINE__);
	    PrintFailedMessage(L"\nOpenStubFileTest failed.\n\n");	   
	}
	else
	{
	   PrintPassedMessage(L"\nOpenStubFileTest Passed.\n\n");
	}

	if( INVALID_HANDLE_VALUE != hFile )
	{
	   CloseHandle(hFile);
	}

	return ret;

}

BOOL
GetTagDataTest()
{
    BOOL ret = FALSE;
	HANDLE hFile = INVALID_HANDLE_VALUE;
	ULONG tagDataLength = (ULONG)strlen(testTagData);
	CHAR* tagData = (CHAR*)malloc(tagDataLength);

	ret = OpenStubFile(unitTestStubFile,GENERIC_READ,SHARE_READ,&hFile);
	if( !ret )
	{
		PrintLastErrorMessage( L"OpenStubFile failed.",__LINE__);
		goto EXIT;
	}

	ret = GetTagData(hFile,&tagDataLength,(BYTE*)tagData);
	if( !ret )
	{
		PrintLastErrorMessage( L"GetTagData failed.",__LINE__);
		goto EXIT;
	}

	if( memcmp(tagData,testTagData,tagDataLength)!= 0)
	{
		PrintFailedMessage(L"Return tag data is not valid.\n");
		ret = FALSE;
	}

EXIT:

	free(tagData);

	if(!ret )
	{
	   PrintFailedMessage(L"\nGetTagDataTest failed.\n\n");	   
	}
	else
	{
	   PrintPassedMessage(L"\nGetTagDataTest Passed.\n\n");
	}

	if( INVALID_HANDLE_VALUE != hFile )
	{
	   CloseHandle(hFile);
	}

	return ret;

}


BOOL
RemoveTagDataTest()
{
    BOOL ret = FALSE;
	HANDLE hFile = INVALID_HANDLE_VALUE;
	ULONG fileAttribute = FILE_ATTRIBUTE_NORMAL;

	//remove readonly if it exists there, please note that the reparse point 
	//attribute can't be removed by this API.
	SetFileAttributes(unitTestStubFile,fileAttribute);

	fileAttribute = GetFileAttributes(unitTestStubFile);
	if(( fileAttribute & FILE_ATTRIBUTE_REPARSE_POINT) == 0)
	{
		ret = FALSE;
		PrintFailedMessage(L"The reparse point attribute was removed.\n");
		goto EXIT;
	}

	ret = OpenStubFile(unitTestStubFile,GENERIC_WRITE,SHARE_READ,&hFile);
	if( !ret )
	{
		PrintLastErrorMessage( L"OpenStubFile failed.",__LINE__);
		goto EXIT;
	}

	ret = RemoveTagData(hFile);
	if( !ret )
	{
		PrintLastErrorMessage( L"RemoveTagData failed.",__LINE__);
		goto EXIT;
	}

	fileAttribute = GetFileAttributes(unitTestStubFile);

	if( (fileAttribute & FILE_ATTRIBUTE_REPARSE_POINT) > 0)
	{
		ret = FALSE;
		PrintFailedMessage(L" The reparse point attribute wasn't removed.\n");
	}

EXIT:

	if(!ret )
	{
	   PrintFailedMessage(L"\nRemoveTagDataTest failed.\n\n");	   
	}
	else
	{
	   PrintPassedMessage(L"\nRemoveTagDataTest Passed.\n\n");
	}

	if( INVALID_HANDLE_VALUE != hFile )
	{
	   CloseHandle(hFile);
	}

	return ret;

}

//run after RemoveTagDataTest finished
BOOL
AddTagDataTest()
{
    BOOL ret = FALSE;
	HANDLE hFile = INVALID_HANDLE_VALUE;
	ret = OpenStubFile(unitTestStubFile,GENERIC_WRITE,SHARE_READ,&hFile);
	if( !ret )
	{
		PrintLastErrorMessage( L"OpenStubFile failed.",__LINE__);
		goto EXIT;
	}

	ret = AddTagData(hFile,(ULONG)strlen(testTagData),(BYTE*)testTagData);
	if( !ret )
	{
		PrintLastErrorMessage( L"AddTagData failed.",__LINE__);
	}

EXIT:

	if(!ret )
	{
	   PrintFailedMessage(L"\nAddTagDataTest failed.\n\n");	   
	}
	else
	{
	   PrintPassedMessage(L"\nAddTagDataTest Passed.\n\n");
	}

	if( INVALID_HANDLE_VALUE != hFile )
	{
	   CloseHandle(hFile);
	}

	return ret;

}

BOOL
ReadStubFile(WCHAR* fileName)
{
	BOOL ret = FALSE;	

	HANDLE pFile = CreateFile(fileName,GENERIC_READ,NULL,NULL,OPEN_EXISTING,FILE_ATTRIBUTE_NORMAL,NULL);
	if( INVALID_HANDLE_VALUE == pFile )
	{
		wprintf(L"Open file %ws failed.",fileName);
		PrintErrorMessage(L"Open file for read test failed.",GetLastError());
		goto EXIT;
	}
	else
	{
		ULONG bufferLength = (ULONG)strlen(testData);
		CHAR* buffer = (CHAR*)malloc( bufferLength );

		if(NULL == buffer)
		{
			PrintErrorMessage(L"Can't allocate memory for test.",0);
			goto EXIT;
		}

		ret = ReadFile(pFile,buffer,bufferLength,&bufferLength,NULL);

		if(0 == ret)
		{
			PrintErrorMessage(L"ReadStubTest failed.",GetLastError());
			goto EXIT;
		}

		if( memcmp(buffer,testData,bufferLength) == 0)
		{

			//
			//The filter driver will call the callback function to restore the stub file on the first read.
			//

			ret = TRUE;
		}		
		else
		{
			wprintf(L"ReadStubTest %ws, data is not correct.",fileName);
			ret = FALSE;
		}
	  
	}

EXIT:	

	if(pFile)
	{
		CloseHandle(pFile);
	}

	return ret;

}


//
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
//
BOOL
ReadStubFileTest()
{	
	//for block read file test, in callback function, we only return the block data to user,
	//the stub file won't change, it is especially good for large file and only need blocks of data.
	if( !ReadStubFile(blockTestStubFile))
	{
		PrintFailedMessage(L"\nRead blockTestStubFile Failed.\n\n");
		return FALSE;
	}
	else
	{
		PrintPassedMessage(L"\nRead blockTestStubFile Passed.\n\n");
	}

	//for cache file file test, in callback function, we will download the whole file to a cache file and return.
	if( !ReadStubFile(wholeFileTestStubFile))
	{
		PrintFailedMessage(L"\nRead wholeFileTestStubFile Failed.\n\n");
		return FALSE;
	}
	else
	{
		PrintPassedMessage(L"\nRead wholeFileTestStubFile Passed.\n\n");
	}

	//for stub file rehydration test, in callback function, we will restore all the data to the stub file,
	//the stub file will turn into normal file, so this is the file level read
	if( !ReadStubFile(rehydrateStubFile))
	{
		PrintFailedMessage(L"\nRead rehydrateStubFile Failed.\n\n");
		return FALSE;
	}
	else
	{
		ULONG fileAttribute = GetFileAttributes(rehydrateStubFile);
		if(( fileAttribute & FILE_ATTRIBUTE_REPARSE_POINT) != 0)
		{
			//the reparsepoint attribute should be removed.
			PrintFailedMessage(L"\nRead rehydrateStubFile Failed, the stub file is not rehydrated.\n\n");
		}
		else
		{
			PrintPassedMessage(L"\nRead rehydrateStubFile Passed.\n\n");
		}
	}


	//for the reparse stub file test, the stub file open will be reparsed to the new target file open.
	if( !ReadStubFile(reparseStubFile))
	{
		PrintFailedMessage(L"\nRead reparseStubFile Failed.\n\n");
		return FALSE;
	}
	else
	{
		PrintPassedMessage(L"\nRead reparseStubFile Passed.\n\n");
	}

	return TRUE;

}



BOOL
EventTest()
{
	BOOL ret = FALSE;	
	DWORD dwTransferred = 0;
	
	DWORD nError = 0;

	if(!CreateDirectory(testEventFolder,NULL))
	{
		DWORD lastError = GetLastError();
		if( lastError != ERROR_ALREADY_EXISTS )
		{
			PrintErrorMessage(L"Create testEventFolder failed.", lastError);
			goto EXIT;
		}
	}

	//create test event file
	HANDLE pFile = CreateFileA(testEventFile,GENERIC_ALL,NULL,NULL,CREATE_ALWAYS,FILE_ATTRIBUTE_NORMAL,NULL);
	if( INVALID_HANDLE_VALUE == pFile )
	{
		PrintErrorMessage(L"Open file for targetTestFile failed.",GetLastError());
		goto EXIT;
	}

	CloseHandle(pFile);
	printf("Create test file %s\n",testEventFile);

	pFile = CreateFileA(testEventFile,GENERIC_ALL,NULL,NULL,OPEN_EXISTING,FILE_ATTRIBUTE_NORMAL,NULL);
	if( INVALID_HANDLE_VALUE == pFile )
	{
		PrintErrorMessage(L"Open file for targetTestFile failed.",GetLastError());
		goto EXIT;
	}
	if(!WriteFile(pFile, testData,(DWORD)strlen(testData), (LPDWORD)&dwTransferred, NULL))
	{
		nError = GetLastError();
		PrintErrorMessage(L"WriteFile failed.", nError);
		ret = FALSE;
		goto EXIT;
	}

	printf("Write data to test file %s\n",testEventFile);

	CloseHandle(pFile);

	int result= rename( testEventFile , testEventRenameFile );
	if ( result == 0 )
		puts ( "File successfully renamed" );
	else
	{
		perror( "Error renaming file" );
		goto EXIT;
	}

   if( remove( testEventRenameFile ) != 0 )
   {
		perror( "Error deleting file" );
		goto EXIT;
   }
   else
		puts( "File successfully deleted" );

   Sleep(2000);

	ret = TRUE;

EXIT:
	return ret;

}
	 
void 
FilterDriverUnitTest()
{
	BOOL ret = FALSE;

	//Reset the filter config setting.
	ret = ResetConfigData();
	if( !ret )
	{
		PrintLastErrorMessage( L"ResetConfigData failed.",__LINE__);
		return;
	}

	//Set filter maiximum wait for user mode response time out.
	ret = SetConnectionTimeout(30); 
	if( !ret )
	{
		PrintLastErrorMessage( L"SetConnectionTimeout failed.",__LINE__);
		return;
	}

	//
	//If you want to monitor the file event(created,changed,renamed,deleted) of the folders,
	//you can register the event here
	//
	ret = RegisterEvent(FILE_CREATEED|FILE_CHANGED|FILE_RENAMED|FILE_DELETED,L"c:\\CloudTierUnitTest\\eventTest\\*");
	if( !ret )
	{
		PrintLastErrorMessage( L"RegisterEvent failed.",__LINE__);
		return ;
	}

	CreateTestSourceFile();

	CreateTestStubFiles();

	OpenStubFileTest();

	GetTagDataTest();

	RemoveTagDataTest();

	AddTagDataTest();

	ReadStubFileTest();

	EventTest();

}