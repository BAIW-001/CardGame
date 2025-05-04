@echo off
cd /d %~dp0
echo 当前目录：%cd%
echo.

:: 获取当前时间（格式：YYYY-MM-DD HH:MM:SS）
for /f "tokens=1-6 delims=:/ " %%a in ("%date% %time%") do (
    set year=%%a
    set month=%%b
    set day=%%c
    set hour=%%d
    set minute=%%e
    set second=%%f
)

:: 格式化时间为：YYYYMMDD_HHMMSS
set timestamp=%year%%month%%day%_%hour%%minute%%second%

:: 提交说明输入
set /p msg=请输入提交说明（例如：修复bug/新增功能）：

:: 拼接说明 + 时间戳
set fullmsg=%msg% [%timestamp%]

echo.
echo 添加文件中...
git add .

echo.
echo 提交中：%fullmsg%
git commit -m "%fullmsg%"

echo.
echo 拉取远程更改...
git pull --rebase origin master

echo.
echo 推送中...
git push

echo.
echo 推送完成！按任意键退出...
pause >nul
