import { useState, useEffect } from 'react';
import { CheckIn, fetchUserInfo } from '@/api/UserInfo';
import { Link } from '@/components/Link/Link.tsx';
import { Cell } from '@telegram-apps/telegram-ui';

interface CheckInStatusProps {
  userId: string;
}

export const CheckInStatus: React.FC<CheckInStatusProps> = ({ userId }) => {
  const [statusMessage, setStatusMessage] = useState<string>('📅 加载中...');
  const [isCheckedInToday, setIsCheckedInToday] = useState(false);

  useEffect(() => {
    const checkStatus = async () => {
      try {
        const userInfo = await fetchUserInfo(userId);
        const lastCheckIn = userInfo?.lastCheckIn;
        if (lastCheckIn) {
          const lastCheckInDate = new Date(
            `${lastCheckIn.substring(0, 4)}-${lastCheckIn.substring(4, 6)}-${lastCheckIn.substring(6, 8)}`
          );
          const today = new Date();
          const diffTime = today.getTime() - lastCheckInDate.getTime();
          const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24));
          if (diffDays == 0) {
            setStatusMessage('📅 今天已经签到过啦，明天再来吧');
            setIsCheckedInToday(true);
          } else if (diffDays >= 1) {
            setStatusMessage(`📅 已经${diffDays}天没有签到啦！(大叫)`);
          }else{
            setStatusMessage(`📅 今天还没有签到！(大叫)`);
          }
        } else {
          setStatusMessage('📅 获取签到状态失败');
        }
      } catch (error) {
        console.error('Error checking sign-in status:', error);
        setStatusMessage('📅 获取签到状态失败');
      }
    };

    checkStatus();
  }, [userId]);

  const handleCheckIn = async () => {
    try {
      const result = await CheckIn(userId);
      let message;
      switch (result) {
        case -2:
          message = '用户不存在';
          break;
        case -1:
          message = '未知错误';
          break;
        case 1:
          message = '✔ 签到成功，余额 +100';
          setStatusMessage('📅 今天已经签到过啦，明天再来吧');
          setIsCheckedInToday(true);
          break;
        case 2:
          message = '✔ 今天已经签到过啦，明天再来吧';
          break;
        default:
          message = '未知错误';
      }
      alert(message);
    } catch (error) {
      console.error('Error during check-in:', error);
      alert('签到失败，请稍后重试');
    }
  };

  return (
    <Link to="#" onClick={handleCheckIn}>
      <Cell disabled={isCheckedInToday}>
        {statusMessage}
      </Cell>
    </Link>
  );
};
