@echo off
cd /d %~dp0
echo 当前目录：%cd%

echo.
set /p msg=请输入提交说明（例如：修复bug/新增功能）：

:: 获取当前时间
for /f "tokens=1-4 delims=/ " %%a in ('date /t') do (
    set y=%%c
    set m=%%a
    set d=%%b
)
for /f "tokens=1-2 delims=: " %%i in ("%time%") do (
    set hh=%%i
    set mm=%%j
)

:: 拼接时间字符串为 YYYY-MM-DD_HH-MM
set timestamp=%y%-%m%-%d%_%hh%-%mm%

:: 拼接最终提交信息
set final_msg=%msg% [%timestamp%]

echo.
echo 添加文件中...
git add .

echo.
echo 提交中...
git commit -m "%final_msg%"

echo.
echo 拉取远程更改中...
git pull --rebase origin master

echo.
echo 推送中...
git push

echo.
echo 提交完成：%final_msg%
pause >nul
