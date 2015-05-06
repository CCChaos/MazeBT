
namespace CCCommon
{

	// Message Code Define
	public enum TMSGCODE
	{
		emSUCCESS = 1,
		
		#region system and logic error (100000-199999)
		emSys_Null = 100000,
		emSys_IndexLeftOverflow = 100001,
		emSys_IndexRightOverflow = 100002,
		emSys_InsertDup = 100003,
		emSys_DeleteEmpty = 100004,
		emSys_UpdateEmpty = 100005,
        emSys_Invalid = 100006,
		#endregion
	
	}
}