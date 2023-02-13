namespace pocstock.Data
{
    public class InitialMasterData
    {
        public const string HistoryStatus = @"
                INSERT INTO [dbo].[HistoryStatus]([HistoryStatusName])
                VALUES('เพิ่มสินค้าเข้าระบบ');
                INSERT INTO [dbo].[HistoryStatus]([HistoryStatusName])
                VALUES('เพิ่มสินค้าในสต๊อก');
                INSERT INTO [dbo].[HistoryStatus]([HistoryStatusName])
                VALUES('ลดสินค้าในสต๊อก');
                INSERT INTO [dbo].[HistoryStatus]([HistoryStatusName])
                VALUES('ลบสิ้นค้าออกจากระบบ');
                INSERT INTO [dbo].[HistoryStatus]([HistoryStatusName])
                VALUES('กู้คืนข้อมูลสินค้า');
                INSERT INTO [dbo].[HistoryStatus]([HistoryStatusName])
                VALUES('สิ้นค้าถูกใช้ในบิล');
                INSERT INTO [dbo].[HistoryStatus]([HistoryStatusName])
                VALUES('สิ้นค้าคืนจากบิลที่แก้ไข');
            ";

        public const string HistoryStatus_SQLite = @"
                INSERT INTO HistoryStatus(HistoryStatusName)
                VALUES('เพิ่มสินค้าเข้าระบบ');
                INSERT INTO HistoryStatus(HistoryStatusName)
                VALUES('เพิ่มสินค้าในสต๊อก');
                INSERT INTO HistoryStatus(HistoryStatusName)
                VALUES('ลดสินค้าในสต๊อก');
                INSERT INTO HistoryStatus(HistoryStatusName)
                VALUES('ลบสิ้นค้าออกจากระบบ');
                INSERT INTO HistoryStatus(HistoryStatusName)
                VALUES('กู้คืนข้อมูลสินค้า');
                INSERT INTO HistoryStatus(HistoryStatusName)
                VALUES('สิ้นค้าถูกใช้ในบิล');
                INSERT INTO HistoryStatus(HistoryStatusName)
                VALUES('สิ้นค้าคืนจากบิลที่แก้ไข');
            ";

        public const string JobStatus = @"
                INSERT INTO [dbo].[JobStatus]([JobStatusName])
                VALUES('รอดำเนินการ');
                INSERT INTO [dbo].[JobStatus]([JobStatusName])
                VALUES('มัดจำรอบที่ 1');
                INSERT INTO [dbo].[JobStatus]([JobStatusName])
                VALUES('มัดจำรอบที่ 2');
                INSERT INTO [dbo].[JobStatus]([JobStatusName])
                VALUES('มัดจำรอบที่ 3');
                INSERT INTO [dbo].[JobStatus]([JobStatusName])
                VALUES('ชำระแล้ว');
            ";

        public const string JobStatus_SQLite = @"
                INSERT INTO JobStatus(JobStatusName)
                VALUES('รอดำเนินการ');
                INSERT INTO JobStatus(JobStatusName)
                VALUES('มัดจำรอบที่ 1');
                INSERT INTO JobStatus(JobStatusName)
                VALUES('มัดจำรอบที่ 2');
                INSERT INTO JobStatus(JobStatusName)
                VALUES('มัดจำรอบที่ 3');
                INSERT INTO JobStatus(JobStatusName)
                VALUES('ชำระแล้ว');
            ";
    }
}
