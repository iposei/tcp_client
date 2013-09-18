using System;

namespace prj301.NetworkLib
{
	public enum ConnState
	{
		EConnecting			= 0,
		EConnectTimeOut 	= 1,
		EConnectError		= 2,
		EConnected			= 3,
		EDisconnected		= 4,
	}
	
	public enum ConnErrorCode
	{
		ECreateSocket   	=   0,
		EConnectToServer	=   1,
		//ESelect         	=   2,
		ERecvZeroByte   	=   3,
		ESendZeroByte   	=   4,
	}
}

