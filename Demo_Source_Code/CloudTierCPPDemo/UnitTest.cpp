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

static CHAR* testEventFile = "c:\\filterTest\\eventTest\\test.txt";
static CHAR* testEventRenameFile = "c:\\filterTest\\eventTest\\rename.txt";
static WCHAR* testEventFolder = L"c:\\filterTest\\eventTest";
static WCHAR* testFolder = L"c:\\filterTest";
static WCHAR* fileReparseTestFolder = L"c:\\filterTest\\fileReparseTestFolder";
static WCHAR* fileRestoreTestFolder = L"c:\\filterTest\\fileRestoreTestFolder";
static WCHAR* blockTestFolder = L"c:\\filterTest\\blockTestFolder";
static WCHAR* cacheFolder = L"c:\\filterTest\\cacheFolder";
static WCHAR* cacheFolderFilterMask = L"c:\\filterTest\\cacheFolder\\*";
static WCHAR* testStubFileFolder = L"c:\\filterTest\\testStubFileFolder";
static WCHAR* reparseLocalTestFile = L"c:\\filterTest\\fileReparseTestFolder\\LocalStubFileTest.txt";
static WCHAR* reparseUNCTestFile = L"c:\\filterTest\\fileReparseTestFolder\\UNCStubFileTest.txt"; //the reparse tag data point to UNC path
static WCHAR* targetTestFile = L"c:\\filterTest\\targetTest.txt";
static WCHAR* targetUNCTestFile = L"\\\\localhost\\c$\\filterTest\\targetTest.txt";
static WCHAR* blockTestFile = L"c:\\filterTest\\blockTestFolder\\StubFileTest.txt";
static WCHAR* testFile = L"c:\\filterTest\\fileRestoreTestFolder\\StubFileTest.txt";
static WCHAR* cacheFile = L"c:\\filterTest\\cacheFolder\\StubFileTest.txt";
static WCHAR* fileRestoreTestFile = L"c:\\filterTest\\fileRestoreTestFolder\\StubFileTest.txt";
static CHAR* testData = "CloudTier test data which when you read the emty stub file, this data will be restored to the file as the file actual content,and the file system will read this data and return it to the user application";
static CHAR* reparseData = "This is reparse point tag data for test in the stub file. The maximum length is 16*1024 bytes.";

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
	return cacheFile;
}

WCHAR*
GetCacheFolder()
{
	return cacheFolder;
}

BOOL
IsRestoreFileTestFolder(WCHAR* fileName )
{
	if( _wcsnicmp(fileRestoreTestFolder,fileName,wcslen(fileRestoreTestFolder)) == 0)
	{
		return TRUE;
	}
	else
	{
		return FALSE;
	}
}

BOOL
IsBlockTestFolder(WCHAR* fileName )
{
	if( _wcsnicmp(blockTestFolder,fileName,wcslen(blockTestFolder)) == 0)
	{
		return TRUE;
	}
	else
	{
		return FALSE;
	}
}

VOID
CreateTestFile()
{
	char* testFileContent = "This is test content for stub file.\r\n";
	WCHAR testFileName[260];

	ZeroMemory(testFileName, sizeof(testFileName));
	for ( ULONG i = 0; i < 10; i++)
	{
		swprintf_s(testFileName,L"%s\\test.%d.txt",cacheFolder,i);

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

		wprintf(L"Created test file %s\r\n",testFileName);

	}

}

VOID
CreateTestStubFiles()
{
	//we will create the stub files based on the cache folder's files.
	//when the user open the stub file in test stub file folder,all data will read from cache folder's file.

	WIN32_FIND_DATA ffd;
	HANDLE pFile = FindFirstFile(cacheFolderFilterMask, &ffd);
	ULONG testStubFileFolderLength = (int)wcslen(testStubFileFolder)*2;

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
		 // L"c:\\filterTest\\testStubFileFolder"  + "\\" + fileName + "\0"

		 ULONG fileNameLength = (ULONG)(testStubFileFolderLength + 2*sizeof(WCHAR) + 2*wcslen(ffd.cFileName));
		 WCHAR* fileName =  (PWCHAR)LocalAlloc(LPTR, fileNameLength);
        if (!fileName)
        {
			PrintLastErrorMessage( L"Allocate memory failed.",__LINE__);
			goto EXIT;
        }

		memset(fileName,0,fileNameLength);

		memcpy(fileName,testStubFileFolder,testStubFileFolderLength);

		fileName[testStubFileFolderLength/2] = (WCHAR)'\\';
		memcpy((PUCHAR)fileName + testStubFileFolderLength + 2,ffd.cFileName,(int)2*wcslen(ffd.cFileName));

		//here we set tag data with the file name in cache folder,in the filter callback function, it then can get back the cache file name for this stub file.
		//in your case, you can set it as the data you need.
		ULONG tagDataLength = (ULONG)2*wcslen(ffd.cFileName);
		ret = CreateStubFile(fileName,fileSize.QuadPart,ffd.dwFileAttributes,tagDataLength,(BYTE*)ffd.cFileName,TRUE,&hFile);

		wprintf(L"Create test stub file %ws return %d.\n",fileName,ret);

		LocalFree(fileName);

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
CreateUnitTestStubFile()
{
	BOOL ret = FALSE;
	HANDLE hFile = INVALID_HANDLE_VALUE;
	LONGLONG fileSize = strlen(testData);
	ULONG fileAttribute = FILE_ATTRIBUTE_NORMAL;
	ULONG tagDataLength = (ULONG)strlen(reparseData);
	
	ret = CreateDirectory(testFolder,NULL);
	if( !ret )
	{
		DWORD lastError = GetLastError();
		if( lastError != ERROR_ALREADY_EXISTS )
		{
			PrintErrorMessage(L"Create testFolder failed.", lastError);
			goto EXIT;
		}
	}	

	ret = CreateDirectory(fileReparseTestFolder,NULL);
	if( !ret )
	{
		DWORD lastError = GetLastError();
		if( lastError != ERROR_ALREADY_EXISTS )
		{
			PrintErrorMessage(L"Create fileReparseTestFolder failed.", lastError);
			goto EXIT;
		}
	}	

	//Create the fileRestoreTestFolder.
 	ret = CreateDirectory(fileRestoreTestFolder,NULL);

	if( !ret )
	{
		DWORD lastError = GetLastError();
		if( lastError != ERROR_ALREADY_EXISTS )
		{
			PrintErrorMessage(L"Create fileRestoreTestFolder failed.", lastError);
			goto EXIT;
		}
	}	

	ret = CreateDirectory(testEventFolder,NULL);
	if( !ret )
	{
		DWORD lastError = GetLastError();
		if( lastError != ERROR_ALREADY_EXISTS )
		{
			PrintErrorMessage(L"Create testEventFolder failed.", lastError);
			goto EXIT;
		}
	}	

	ret = CreateDirectory(cacheFolder,NULL);
	if( !ret )
	{
		DWORD lastError = GetLastError();
		if( lastError != ERROR_ALREADY_EXISTS )
		{
			PrintErrorMessage(L"Create cacheFolder failed.", lastError);
			goto EXIT;
		}
	}

	ret = CreateDirectory(testStubFileFolder,NULL);
	if( !ret )
	{
		DWORD lastError = GetLastError();
		if( lastError != ERROR_ALREADY_EXISTS )
		{
			PrintErrorMessage(L"Create test stub file folder failed.", lastError);
			goto EXIT;
		}
	}


	hFile = CreateFile(
						cacheFile, 
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
			PrintErrorMessage(L"Create cacheFile failed.", lastError);
			goto EXIT;
		}		
	}


	ULONG bytesWritten = 0;
	ret = WriteFile(hFile,GetTestData(),(DWORD)strlen(GetTestData()),&bytesWritten,NULL);
	if(!ret)
	{
		PrintErrorMessage(L"WriteFile failed.",GetLastError());
		goto EXIT;
	}

	if( INVALID_HANDLE_VALUE != hFile )
	{
	   CloseHandle(hFile);
	}

	ret = CreateStubFile(fileRestoreTestFile,fileSize,fileAttribute,tagDataLength,(BYTE*)reparseData,TRUE,&hFile);
	if( !ret )
	{
		PrintLastErrorMessage( L"CreateStubFile failed.",__LINE__);
		goto EXIT;
	}

	if( INVALID_HANDLE_VALUE != hFile )
	{
	   CloseHandle(hFile);
	}

	//Create the blockTestFolder.
 	ret = CreateDirectory(blockTestFolder,NULL);

	if( !ret )
	{
		DWORD lastError = GetLastError();
		if( lastError != ERROR_ALREADY_EXISTS )
		{
			PrintErrorMessage(L"Create blockTestFolder failed.", lastError);
			goto EXIT;
		}
	}	

	ret = CreateStubFile(blockTestFile,fileSize,fileAttribute,tagDataLength,(BYTE*)reparseData,TRUE,&hFile);
	if( !ret )
	{
		PrintLastErrorMessage( L"CreateStubFile failed.",__LINE__);
	}


EXIT:

	if(!ret )
	{
	   PrintFailedMessage(L"\nCreateStubFileTest failed.\n\n");	   
	}
	else
	{
	   PrintPassedMessage(L"\nCreateStubFileTest Passed.\n\n");
	}

	if( INVALID_HANDLE_VALUE != hFile )
	{
	   CloseHandle(hFile);
	}

	return ret;

}

BOOL
OpenStubFileTest()
{
    BOOL ret = FALSE;
	HANDLE hFile = INVALID_HANDLE_VALUE;

	ret = OpenStubFile(testFile,GENERIC_READ,SHARE_READ,&hFile);
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
	ULONG tagDataLength = (ULONG)strlen(reparseData);
	CHAR* tagData = (CHAR*)malloc(tagDataLength);

	ret = OpenStubFile(testFile,GENERIC_READ,SHARE_READ,&hFile);
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

	if( memcmp(tagData,reparseData,tagDataLength)!= 0)
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
	SetFileAttributes(testFile,fileAttribute);

	fileAttribute = GetFileAttributes(testFile);
	if(( fileAttribute & FILE_ATTRIBUTE_REPARSE_POINT) == 0)
	{
		ret = FALSE;
		PrintFailedMessage(L"The reparse point attribute was removed.\n");
		goto EXIT;
	}

	ret = OpenStubFile(testFile,GENERIC_WRITE,SHARE_READ,&hFile);
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

	fileAttribute = GetFileAttributes(testFile);

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
	ret = OpenStubFile(testFile,GENERIC_WRITE,SHARE_READ,&hFile);
	if( !ret )
	{
		PrintLastErrorMessage( L"OpenStubFile failed.",__LINE__);
		goto EXIT;
	}

	ret = AddTagData(hFile,(ULONG)strlen(reparseData),(BYTE*)reparseData);
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
ReadTest(WCHAR* fileName)
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

	if( ret )
	{
		PrintPassedMessage(L"\nReadStubTest Passed.\n\n");
	}
	else
	{
		PrintFailedMessage(L"\nReadStubTest Failed.\n\n");
	}

	if(pFile)
	{
		CloseHandle(pFile);
	}

	return ret;

}

BOOL
ReadStubFileTest()
{
	//for restore file test, in callback function, we will restore all the data to the stub file,
	//the stub file will turn into normal file, so this is the file level read
	if( !ReadTest(fileRestoreTestFile))
	{
		return FALSE;
	}

	//for block file test, in callback function, we only return the block data to user,
	//the stub file won't change, it is especially good for large file and only need blocks of data.
	if( !ReadTest(blockTestFile))
	{
		return FALSE;
	}

	return TRUE;

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

	ret = CreateStubFile(StubFileName,strlen(testData),FILE_ATTRIBUTE_NORMAL,reparseTagDataLength,(BYTE*)reparseTagData,TRUE,&pFile);
	if( !ret )
	{
		PrintLastErrorMessage( L"CreateStubFile failed.",__LINE__);
	}
EXIT:
	
	CloseHandle(pFile);

	return ret;
}

//
//Set the repare tag data as your reparse file name
//when the user open the stub file, the filter driver will get the reparse file name from the tag data.
//then reparse the open to this new file name.But make sure the user has the permission to the new file name.
//
BOOL
RepareFileTest()
{
	BOOL ret = FALSE;	
	DWORD dwTransferred = 0;
	
	DWORD nError = 0;

	//create target file
	HANDLE pFile = CreateFile(targetTestFile,GENERIC_ALL,NULL,NULL,OPEN_ALWAYS,FILE_ATTRIBUTE_NORMAL,NULL);
	
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

	CloseHandle(pFile);

	ret = CreateReparseStubFile(reparseLocalTestFile,targetTestFile);
	if(!ret)
	{
		goto EXIT;
	}

	ret = ReadTest(reparseLocalTestFile);
	if(!ret)
	{
		goto EXIT;
	}

	ret = CreateReparseStubFile(reparseUNCTestFile,targetUNCTestFile);
	if(!ret)
	{
		goto EXIT;
	}

	ret = ReadTest(reparseUNCTestFile);
	if(!ret)
	{
		goto EXIT;
	}

EXIT:

	if( ret )
	{
		PrintPassedMessage(L"\nRepareFileTest Passed.\n\n");
	}
	else
	{
		PrintFailedMessage(L"\nRepareFileTest Failed.\n\n");
	}

	if(pFile)
	{
		CloseHandle(pFile);
	}

	return ret;
}

BOOL
EventTest()
{
	BOOL ret = FALSE;	
	DWORD dwTransferred = 0;
	
	DWORD nError = 0;

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

	//when ENABLE_NO_RECALL_FLAG was set, the filter driver will skip the reparse point file with 
	//open option FILE_FLAG_OPEN_NO_RECALL
	ret = SetBooleanConfig(ENABLE_NO_RECALL_FLAG);
	if( !ret )
	{
		PrintLastErrorMessage( L"SetBooleanConfig failed.",__LINE__);
		return ;
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
	ret = RegisterEvent(FILE_CREATEED|FILE_CHANGED|FILE_RENAMED|FILE_DELETED,L"c:\\filterTest\\eventTest\\*");
	if( !ret )
	{
		PrintLastErrorMessage( L"RegisterEvent failed.",__LINE__);
		return ;
	}


	CreateUnitTestStubFile();

	CreateTestFile();

	CreateTestStubFiles();

	OpenStubFileTest();

	GetTagDataTest();

	RemoveTagDataTest();

	AddTagDataTest();

	ReadStubFileTest();

	RepareFileTest();	

	EventTest();

}