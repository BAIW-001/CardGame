@echo off
cd /d %~dp0
echo ��ǰĿ¼��%cd%

echo.
set /p msg=�������ύ˵�������磺�޸�bug/�������ܣ���

echo.
rem ��ȡ��ǰ����ʱ�䣨��ʽ��yyyy-MM-dd_HH-mm��
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

echo ��ǰʱ���: %timestamp%

echo.
echo ��������ļ�...
git add .

echo.
echo �����ύ...
git commit -m "[%timestamp%] %msg%"

echo.
echo ������ȡԶ�̸���...
git pull --rebase origin master

echo.
echo �������͵� GitHub...
git push

echo.
echo ��ɣ���������˳�...
pause >nul
