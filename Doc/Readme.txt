CloudTier Demo ReadMe

CloudTier demo is a c# Windows forms application, to demo the transparent storage tiering SDK, it will demo how the storage was tiered, 
how to automatically move data between high-cost and low-cost storage media. 

How to run the CloudTier demo?
1.	Create the stub files first, go to tools->create stub test files.
2.	The test source folder stores the test file data to simulate the remote host server. 
3.	The test stub file folder stores the test stub files, the stub file doesn’t take the storage space.
4.	After the stub files were created, start the filter service. The stub files can be read as a regular file, when you open the stub file, 
	all data will read from the source file by the demo application.
5.	For demo purpose, the new stub file’s reparse point tag always pointing to the source file, you can change it to your remote sever, or in the cloud.

