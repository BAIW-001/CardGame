@echo off
cd /d %~dp0
echo ��ǰĿ¼��%cd%
echo.

:: ��ȡ��ǰʱ�䣨��ʽ��YYYY-MM-DD HH:MM:SS��
for /f "tokens=1-6 delims=:/ " %%a in ("%date% %time%") do (
    set year=%%a
    set month=%%b
    set day=%%c
    set hour=%%d
    set minute=%%e
    set second=%%f
)

:: ��ʽ��ʱ��Ϊ��YYYYMMDD_HHMMSS
set timestamp=%year%%month%%day%_%hour%%minute%%second%

:: �ύ˵������
set /p msg=�������ύ˵�������磺�޸�bug/�������ܣ���

:: ƴ��˵�� + ʱ���
set fullmsg=%msg% [%timestamp%]

echo.
echo ����ļ���...
git add .

echo.
echo �ύ�У�%fullmsg%
git commit -m "%fullmsg%"

echo.
echo ��ȡԶ�̸���...
git pull --rebase origin master

echo.
echo ������...
git push

echo.
echo ������ɣ���������˳�...
pause >nul
