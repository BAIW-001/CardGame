@echo off
cd /d %~dp0
echo ��ǰĿ¼��%cd%

echo.
set /p msg=�������ύ˵�������磺�޸�bug/�������ܣ���

:: ��ȡ��ǰʱ��
for /f "tokens=1-4 delims=/ " %%a in ('date /t') do (
    set y=%%c
    set m=%%a
    set d=%%b
)
for /f "tokens=1-2 delims=: " %%i in ("%time%") do (
    set hh=%%i
    set mm=%%j
)

:: ƴ��ʱ���ַ���Ϊ YYYY-MM-DD_HH-MM
set timestamp=%y%-%m%-%d%_%hh%-%mm%

:: ƴ�������ύ��Ϣ
set final_msg=%msg% [%timestamp%]

echo.
echo ����ļ���...
git add .

echo.
echo �ύ��...
git commit -m "%final_msg%"

echo.
echo ��ȡԶ�̸�����...
git pull --rebase origin master

echo.
echo ������...
git push

echo.
echo �ύ��ɣ�%final_msg%
pause >nul
