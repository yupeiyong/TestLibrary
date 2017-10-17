USE [msdb]
GO

/****** Object:  Job [完整备份]    Script Date: 08/07/2017 18:57:56 ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]]    Script Date: 08/07/2017 18:57:56 ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'完整备份', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'无描述。', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'sa', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [收缩文件]    Script Date: 08/07/2017 18:57:57 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'收缩文件', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=3, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'USE master 
GO
--查询所有数据库名称并保存在临时表
SELECT  Name INTO #TEMP FROM Master..SysDatabases ORDER BY Name

DECLARE @db_name AS NVARCHAR(50)
--声明游标
DECLARE CUR CURSOR  FOR SELECT * FROM #TEMP
--打开游标
OPEN CUR
--读取第一条数据
FETCH NEXT FROM CUR INTO @db_name
WHILE @@FETCH_STATUS=0
BEGIN
	IF @db_name<>''tempdb'' AND @db_name<>''model'' AND @db_name<>''msdb''
	BEGIN
		--备份语句
		DECLARE @sql AS NVARCHAR(MAX)
		--备份路径
		DECLARE @disk AS NVARCHAR(MAX)
		--收缩数据库文件
		SET @sql=''
			USE [''+@db_name+'']
			DBCC SHRINKFILE (''''''+@db_name+'''''',0,TRUNCATEONLY);''
		--执行完整备份
		EXEC(@sql)	
	END
	FETCH NEXT FROM CUR INTO @db_name
END
--关闭游标
CLOSE CUR
--释放游标
DEALLOCATE CUR
--删除临时表
DROP TABLE #TEMP', 
		@database_name=N'master', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [完整备份]    Script Date: 08/07/2017 18:57:57 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'完整备份', 
		@step_id=2, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=3, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'USE master 
GO
--查询所有数据库名称并保存在临时表
SELECT  Name INTO #TEMP FROM Master..SysDatabases ORDER BY Name

DECLARE @db_name AS NVARCHAR(50)
--声明游标
DECLARE CUR CURSOR  FOR SELECT * FROM #TEMP
--打开游标
OPEN CUR
--读取第一条数据
FETCH NEXT FROM CUR INTO @db_name
WHILE @@FETCH_STATUS=0
BEGIN
	print @db_name
	IF @db_name=''Site_JunZiMeiRong''
	BEGIN
		--备份语句
		DECLARE @sql AS NVARCHAR(MAX)
		--备份路径
		DECLARE @disk AS NVARCHAR(MAX)
		--备份设备
		SET @disk=''D:\sqlback\''+@db_name+''_FullBackup_''+REPLACE( CONVERT(NVARCHAR(20),GETDATE()-1,120)+''.bak'','':'',''_'')
		--备份语句
		--执行备份，非覆盖非追加不重新格式备份并显示备份进度
		SET @sql=''
			BACKUP DATABASE ''+@db_name+'' 
				TO DISK=''''''+@disk+''''''
				 WITH INIT,NOFORMAT,SKIP,STATS = 10 ''
		PRINT @sql
		--执行完整备份
		EXEC(@sql)
	END
	FETCH NEXT FROM CUR INTO @db_name
END
--关闭游标
CLOSE CUR
--释放游标
DEALLOCATE CUR
--删除临时表
DROP TABLE #TEMP', 
		@database_name=N'master', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'完整备份', 
		@enabled=1, 
		@freq_type=4, 
		@freq_interval=1, 
		@freq_subday_type=1, 
		@freq_subday_interval=0, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=0, 
		@active_start_date=20170527, 
		@active_end_date=99991231, 
		@active_start_time=13000, 
		@active_end_time=235959, 
		@schedule_uid=N'759e0d1a-706e-4fc0-9200-30f701180082'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:

GO


