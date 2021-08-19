# CloudTier Storage Tering SDK
A storage tiering example was implemented with CloudTier Storage Tiering SDK. The CloudTier Storage Tiering SDK provides you a simple solution to develop the cloud archiving software, it allows you to integrate your existing on-premises applications with the remote cloud storage infrastructure in a seamless, secure, and transparent fashion. The cloud tiering is a data storage technique which automatically moves data between high-cost on-premise storage and low-cost remote cloud storage, it provides a native cloud storage tier, it allows you to free up on-premise storage capacity, by moving out cooler data to the cloud storage, thereby reducing capital and operational expenditures.

![loudTier Storage Tiering Architecture](https://www.easefilter.com/images/CloudTiering.png)

# Cloud Archiving Solution for Unstructured Data

Cloud archiving is the process of moving data to secondary storage in the cloud, the potential benefits of cloud archiving include lower costs and easier access, no interruption and change to your existing applications and infrastructure. Automatically archive, manage and secure all your organization’s files to the cloud, transparently access your archived

![loudTier Storage Tiering Solution](https://www.easefilter.com/images/CloudTier.png)

## CloudTier example is a simple C# Windows forms application, to demo how to use the transparent storage tiering SDK. 
The example can generate some stub files. To handle the read request of the stub file, we need to register the callback function for the file system filter driver. When the stub file was accessed, the callback function will be invoked, the callback function will retrieve the data from the remote server and send back to the filter driver.

## How to run the CloudTier demo?

1.	Create the stub files first, go to tools->create stub test files.
	By creating the stub file, you can move out your data to low-cost remote storage, 
        
2.	The test source folder stores the file data to simulate the remote host server. 
	You can store your source file data anywhere, i.e. Amazon s3, Azure storage or your own private cloud storage.

3.	The test stub file folder stores the test stub files.
	The stub file doesn't take the storage space, it only keeps the meta data you created.

4.	After the stub files were created, start the filter service. The stub files can be read as a regular file, when you open the stub file, 
	all data will read from the source file by the demo application.

5.	For demo purpose, the new stub file’s reparse point tag always pointing to the source file, you can change it to your remote sever, or in the cloud.

[Read more about CloudTier SDK example](https://www.easefilter.com/Forums_Files/CloudTier.htm)
