USE [BudAllBackup]
GO

/****** Object:  StoredProcedure [dbo].[sp_GetTransferList2]    Script Date: 06/25/2014 10:55:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Batch submitted through debugger: SQLQuery36.sql|2|0|C:\Users\Administrator\AppData\Local\Temp\~vs3F33.sql
/****** Object:  StoredProcedure [dbo].[sp_GetTransferList2]    Script Date: 04/04/2014 16:18:28 ******/

-- Batch submitted through debugger: SQLQuery31.sql|7|0|C:\Users\Administrator\AppData\Local\Temp\~vs42C.sql
-- Batch submitted through debugger: SQLQuery16.sql|7|0|C:\Users\Administrator\AppData\Local\Temp\~vsBC8E.sql
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--//@disPlayFlg 0:リアルタイム表示; 1:ファイル表示
--@TranferFlg 0:転送中-完了両方;　1:転送中のみ
--@StateFlg 0:OK-NG両方;1:NGのみ
--@LogFlg 0:ログ;1:容量
-- =============================================
alter PROCEDURE [dbo].[sp_GetTransferList2](@DBServerIP varchar(255)= null, @groupId int=0,
	@StartDate datetime=null,@endDate datetime=null,
	@StartTime varchar(20)=null,@endTime varchar(20)=null,
	@TranferFlg int =0,@StateFlg int = 0,
	@LogFlg int = 0,@backupServerFileName VARCHAR(100)=null,
	@Pindex int = 1,@Psize int = 20
	-- Add the parameters for the stored procedure here
	)
AS
begin
	-- set NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	set NOCOUNT ON;

    -- Insert statements for procedure here
    if object_id('tempdb.dbo.#TranferList') is not null     drop table #TranferList
    if object_id('tempdb.dbo.#tempLog') is not null     drop table #tempLog
    if object_id('tempdb.dbo.#tempCount') is not null     drop table #tempCount
    --転送容量表示用の臨時表
    create table #TranferList
    (
    transferDate datetime,
    transferTime int,
    transferFileCount int,
    transferFileSize bigint,
    )
    --ログ表示用の臨時表
    create table #tempLog
    (
    [id] int,
    [DBServerIP] nvarchar(40)  ,
	[monitorServerID] int  ,
	[monitorFileName] nvarchar(255)  ,
	[monitorFilePath] nvarchar(255)  ,
	[monitorFileType] nvarchar(255)  ,
	[monitorFileSize] nvarchar(255)  ,
	[monitorTime] datetime  ,
	[transferFlg] smallint  ,
	[backupServerGroupID] int NULL ,
	[backupServerID] int NULL ,
	[backupServerFileName] nvarchar(50)  ,
	[backupServerFilePath] nvarchar(50)  ,
	[backupServerFileType] nvarchar(50)  ,
	[backupServerFileSize] nvarchar(50)  ,
	[backupStartTime] datetime  ,
	[backupendTime] datetime  ,
	[backupTime] nvarchar(50)  ,
	[backupFlg] smallint  ,
	[copyStartTime] datetime  ,
	[copyendTime] datetime  ,
	[copyTime] nvarchar(255) NULL ,
	[copyFlg] smallint  ,
	[deleteFlg] smallint  ,
	[deleter] nvarchar(255) NULL ,
	[deleteDate] datetime NULL ,
	[creater] nvarchar(255)  ,
	[createDate] datetime  ,
	[updater] nvarchar(255)  ,
	[updateDate] datetime  ,
	[restorer] nvarchar(255) NULL ,
	[restoreDate] datetime NULL,
	[monitorFileStatus] text null,
	[row] int
    )
    create table #tempCount
    (
		[totalCount] int
    )

    declare @sql  varchar(8000)
    declare @sql_count varchar(8000)
    declare @where varchar(8000)
    declare @start_date datetime
    declare @end_date datetime
    declare @num_sql varchar(8000)
    declare @num_where varchar(8000)
    
    set @num_where = ' WHERE row > ' + Convert(varchar(10),((@Pindex-1) * @Psize)) + ' and row <= ' + Convert(varchar(10),(@Pindex * @Psize))
    
    --set @sql = 'SELECT id, backupServerFileName,backupServerFileSize,copyStartTime,copyendTime,backupStartTime,backupendTime,backupTime,backupFlg FROM log '
    set @sql = 'SELECT id, DBServerIP, monitorServerID ,monitorFileName ,monitorFilePath ,monitorFileType ,' +
			'monitorFileSize ,monitorTime ,transferFlg ,backupServerGroupID ,backupServerID ,'+
			'backupServerFileName ,backupServerFilePath ,backupServerFileType ,backupServerFileSize ,'+
			'backupStartTime ,backupendTime ,backupTime ,backupFlg ,copyStartTime ,copyendTime ,'+
			'copyTime ,copyFlg  ,deleteFlg,deleter ,deleteDate ,creater ,createDate ,updater ,updateDate ,'+
			'restorer,restoreDate,monitorFileStatus,ROW_NUMBER() over(order by backupStartTime desc) as row  FROM log'
	
	set @where = ''
    set @where = @where + ' WHERE '
    
    --IF(@TranferFlg = 0)
	--begin
	--転送中-完了両方
		--set @where = @where + ' transferFlg<>2 '  
	--end
    --ELSE
	--begin
	 --転送中のみ
		--set @where = @where + ' transferFlg=0 ' 
	--end
	IF(@StateFlg = 2)
	begin
	--OK・NG両方
		set @where = @where + ' backupFlg<>2'  
	end
	ELSE
	begin
	 --OK・NGのみ
		set @where = @where + ' backupFlg=' + convert(varchar(10),@StateFlg)
	end
	IF(@groupId > 0)
	begin
	--グループID
		set @where = @where + ' AND backupServerGroupID=' + convert(varchar(10),@groupId)
	end
	IF(@backupServerFileName is not null and @backupServerFileName != '')
	begin
		set @where =@where + ' AND (backupServerFileName like ''%' + @backupServerFileName + '%'' OR backupServerFilePath like ''%' + @backupServerFileName + '%'') '  
		--set @where = @where + ' AND (backupServerFileName like ''%' + @backupServerFileName + '%'')'
	end
	--Related IP
	IF(@DBServerIP is not null and @DBServerIP != '')
	begin
		set @where =@where + ' AND DBServerIP in (' + @DBServerIP + ') '
	end
	--IF(@StartDate is null or @StartDate = '')
	--begin
	--開始日付を入力しない場合
		--declare @tempSql nvarchar(100)
		--set @tempSql = 'SELECT @minDate=MIN(backupStartTime) FROM log ' + @where
		--EXEC sp_executesql @tempSql,N'@minDate varchar(20) output',@StartDate output
		--IF(@StartDate is null or @StartDate = '')
		--begin
			--set @StartDate = DATEADD(day,-30, GETDATE())
		--end
		--ELSE
		--begin
			--set @StartDate = CONVERT(varchar(10),@StartDate,120)
		--end
	--end
	--IF(@endDate is null or @endDate='')
	--begin
	--終了日付を入力しない場合
		--set @endDate = GETDATE()
	--end
	IF(@StartTime is null or @StartTime='')
	--開始時間を入力しない場合
	begin
		set @StartTime ='00:00:00'
	end
	IF(@endTime is null or @endTime='')
	begin
	--終了時間を入力しない場合
		set @endTime = '23:59:59'
	end
	declare @sumDays int
	declare @countDays int
	set @sumDays = DATEDIFF(day,@StartDate,@endDate)
	declare @varstart varchar(20)
	declare @varend varchar(20)
	declare @ORwhere varchar(8000)
	set @ORwhere = ''
	IF(@sumDays > 0)
	begin
		--開始日付と終了日付の条件
		--WHILE(@sumDays >= 0)
		--begin
		--終了日付>開始日付の場合
			set @varstart = CONVERT(varchar(10),DATEADD(day,-@sumDays,@endDate),120) +' '+ @StartTime
			--set @varend = CONVERT(varchar(10),DATEADD(day,-@sumDays,@endDate),120) +' '+ @endTime;
			set @varend = CONVERT(varchar(10),@endDate,120) +' '+ @endTime;
			IF(@ORwhere = '')
			begin
				set @ORwhere = @ORwhere + ' (backupStartTime BETWEEN ''@varstart'' AND ''@varend'') '
			end
			ELSE
			begin
				set @ORwhere = @ORwhere + ' OR (backupStartTime BETWEEN ''@varstart'' AND ''@varend'') '
			end
			set @ORwhere = REPLACE(@ORwhere,'@varstart',@varstart)
			set @ORwhere = REPLACE(@ORwhere,'@varend',@varend)
			set @sumDays = @sumDays -1
		--end
	end
	ELSE IF(@sumDays = 0 AND @StartDate!='' AND @endDate!='')
	begin
		--終了日付＝開始日付の場合
		set @varstart = CONVERT(varchar(10),@StartDate,120) +' '+ @StartTime
		set @varend = CONVERT(varchar(10),@endDate,120) +' '+ @endTime;
		set @where = @where + 'AND backupStartTime BETWEEN ''@varstart'' AND ''@varend'' '
		set @where = REPLACE(@where,'@varstart',@varstart)
		set @where = REPLACE(@where,'@varend',@varend)
	end
	IF(@ORwhere !='')
	begin
		set @where = @where + ' AND (' + @ORwhere + ')'
	end
	--Add order phrase 2014-06-05
	BEGIN -- ORDER BY 
		declare @index int
		declare @orderStr varchar(500)
		declare @lastPos int
		declare @length int
		declare @num int
		set @num = 0;

		set @orderStr = ' order by case ';

		set @index = charindex(',',@DBServerIP);
		set @lastPos = 0;
		while 1=1
		begin
			if @index = 0
				set @length = LEN(@DBServerIP)
			else
				set @length = @index - @lastPos - 1;
				
		  set @orderStr += ' when DBServerIP = '+SUBSTRING(@DBServerIP,@lastPos+1,@length)+ ' then ' + cast(@num as varchar)
		  set @num += 1;
		  set @lastPos = @index
		  set @index = charindex(',',@dbserverip,@index+1);
		  if @index = 0 break
		  
		end
		
		if @lastPos != 0
			set @orderStr += ' when DBServerIP = '+SUBSTRING(@DBServerIP,@lastPos+1,@lastPos)+ ' then ' + cast(@num as varchar)

		set @orderStr += ' end ';
	END
	--条件によって、検索データを臨時表にインサート
	set @sql = 'select * from (' + @sql + @where +') as temp '+@num_where
	
	insert into #tempLog(id, DBServerIP, monitorServerID ,monitorFileName ,monitorFilePath ,monitorFileType ,
	monitorFileSize ,monitorTime ,transferFlg ,backupServerGroupID ,backupServerID ,backupServerFileName ,
	backupServerFilePath ,backupServerFileType ,backupServerFileSize ,backupStartTime ,backupendTime ,
	backupTime ,backupFlg,copyStartTime ,copyendTime ,copyTime ,copyFlg  ,deleteFlg,deleter ,deleteDate ,
	creater ,createDate ,updater ,updateDate ,restorer,restoreDate,monitorFileStatus,[row]
	) exec(@sql)
	
	set @sql_count = 'SELECT COUNT(id) as totalCount FROM log ' + @where
	
	insert into #tempCount(totalCount) exec(@sql_count)
	
	IF (@LogFlg=0)
	begin
	--ログ表示の場合
		set @num_sql = 'select *,(select totalCount from #tempCount) as totalCount from #tempLog '
		exec(@num_sql +@orderStr)
	end
	ELSE
	begin
	--転送容量表示の場合
		declare @startHour int
		declare @endHour int
		declare @hourcount int
		set @startHour = DATEPART(HH,@StartTime)
		set @endHour = DATEPART(HH,@endTime)
		set @sumDays = DATEDIFF(day,@StartDate,@endDate)
		WHILE(@sumDays >= 0)
		begin
			set @hourcount = @endHour - @startHour
			WHILE(@hourcount >=0)
			begin
				insert into #TranferList(transferDate,transferTime,transferFileCount,transferFileSize)
				values(CONVERT(varchar(10),DATEADD(day,@sumDays,@StartDate),120),@endHour-@hourcount,0,0)
				set @hourcount = @hourcount -1
			end
			set @sumDays = @sumDays -1
		end
		--select * from #TranferList
		insert into #TranferList(transferDate,transferTime,transferFileCount,transferFileSize)
		SELECT CONVERT(varchar(10),backupStartTime,111),
		CASE DATEPART(HH,backupStartTime)
			WHEN '0' then '0'
			WHEN '1' then '1'
			WHEN '2' then '2'
			WHEN '3' then '3'
			WHEN '4' then '4'
			WHEN '5' then '5'
			WHEN '6' then '6'
			WHEN '7' then '7'
			WHEN '8' then '8'
			WHEN '9' then '9'
			WHEN '10' then '10'
			WHEN '11' then '11'
			WHEN '12' then '12'
			WHEN '13' then '13'
			WHEN '14' then '14'
			WHEN '15' then '15'
			WHEN '16' then '16'
			WHEN '17' then '17'
			WHEN '18' then '18'
			WHEN '19' then '19'
			WHEN '20' then '20'
			WHEN '21' then '21'
			WHEN '22' then '22'
			WHEN '23' then '23'
		end,
		CASE 
			WHEN backupServerFileName is not null THEN 1
			--WHEN backupServerFileName !='' THEN 1
			ELSE 0
		end,
		backupServerFileSize	
		FROM #tempLog
		SELECT convert(varchar(10),transferDate,111) AS 'transferDate',transferTime,sum(transferFileCount) AS 'transferFileCount',sum(transferFileSize) AS 'transferFileSize' FROM #TranferList
		GROUP BY transferDate,transferTime
		order by transferDate,transferTime
	end
	if object_id('tempdb.dbo.#TranferList') is not null     drop table #TranferList
	if object_id('tempdb.dbo.#tempLog') is not null     drop table #tempLog
	
end




GO


