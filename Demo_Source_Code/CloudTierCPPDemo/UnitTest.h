///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2011 EaseFilter Technologies Inc.
//    All Rights Reserved
//
//    This software is part of a licensed software product and may
//    only be used or copied in accordance with the terms of that license.
//
///////////////////////////////////////////////////////////////////////////////

#pragma once

// Defines for NTSTATUS
typedef long NTSTATUS;

void 
FilterDriverUnitTest();

CHAR*
GetTestData();

BOOL
IsRehydrateTestFile(WCHAR* fileName );

BOOL
IsBlockTestFile(WCHAR* fileName );

BOOL
IsRestoreCacheTestFile(WCHAR* fileName );

WCHAR*
GetCacheFile();

WCHAR*
GetCacheFolder();

