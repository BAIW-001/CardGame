@echo off
cd /d %~dp0
echo 当前目录：%cd%

echo.
set /p msg=请输入提交说明（例如：修复bug/新增功能）：

rem 获取原始系统日期
for /f "tokens=1-3 delims=-/ " %%a in ("%date%") do (
    set d1=%%a
    set d2=%%b
    set d3=%%c
)

rem 判断哪项是年份
if %d1% GTR 999 (
    set yyyy=%d1%
    set mm=%d2%
    set dd=%d3%
) else if %d3% GTR 999 (
    set yyyy=%d3%
    set mm=%d2%
    set dd=%d1%
) else (
    set yyyy=%d2%
    set mm=%d1%
    set dd=%d3%
)

rem 去除非数字字符
for /f "delims=0123456789" %%a in ("%mm%") do set mm=%mm:%%a=%
for /f "delims=0123456789" %%a in ("%dd%") do set dd=%dd:%%a=%

rem 补零处理
if 1%mm% LSS 110 set mm=0%mm%
if 1%dd%  LSS 110 set dd=0%dd%

rem 获取时间
for /f "tokens=1-2 delims=: " %%a in ("%time%") do (
    set hh=%%a
    set min=%%b
)

rem 去除空格
set hh=%hh: =%
set min=%min: =%

rem 补零
if 1%hh% LSS 110 set hh=0%hh%
if 1%min% LSS 110 set min=0%min%

rem 拼接时间戳
set timestamp=%yyyy%-%mm%-%dd%_%hh%:%min%
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
echo ? 完成！按任意键退出...
pause >nul
