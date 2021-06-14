# CloudTierSDK
A storage tiering SDK example was implemented with CloudTier Storage Tiering SDK. The CloudTier Storage Tiering SDK provides you a simple solution to develop the cloud archiving software, it allows you to integrate your existing on-premises applications with the remote cloud storage infrastructure in a seamless, secure, and transparent fashion. The cloud tiering is a data storage technique which automatically moves data between high-cost on-premise storage and low-cost remote cloud storage, it provides a native cloud storage tier, it allows you to free up on-premise storage capacity, by moving out cooler data to the cloud storage, thereby reducing capital and operational expenditures.

![loudTier Storage Tiering Architecture](https://www.easefilter.com/images/CloudTiering.png)

The example can generate some stub files. To handle the read request of the stub file, we need to register the callback function for the file system filter driver. When the stub file was accessed, the callback function will be invoked, the callback function will retrieve the data from the remote server and send back to the filter driver.
[Read more about CloudTier SDK example](https://www.easefilter.com/Forums_Files/CloudTier.htm)
