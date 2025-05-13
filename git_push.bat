@echo off
cd /d %~dp0
echo 当前目录：%cd%

echo.
set /p msg=请输入提交说明（例如：修复bug/新增功能）：

echo.
rem 获取当前日期时间（格式：yyyy-MM-dd_HH-mm）
for /f "tokens=1-4 delims=/ " %%a in ('date /t') do (
    set day=%%a
    set month=%%b
    set year=%%c
)
for /f "tokens=1-2 delims=: " %%a in ('time /t') do (
    set hour=%%a
    set minute=%%b
)
set timestamp=%year%-%month%-%day%_%hour%-%minute%

echo 当前时间戳: %timestamp%

echo.
echo 正在添加文件...
git add .

echo.
echo 正在提交...
git commit -m "[%timestamp%] %msg%"

echo.
echo 正在拉取远程更新...
git pull --rebase origin master

echo.
echo 正在推送到 GitHub...
git push

echo.
echo 完成！按任意键退出...
pause >nul
