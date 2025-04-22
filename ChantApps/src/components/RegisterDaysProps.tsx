import { FC, useState, useEffect } from 'react';
import { fetchUserInfo } from '@/api/UserInfo';

interface RegisterDaysProps {
  userId: string;
}

export const RegisterDays: FC<RegisterDaysProps> = ({ userId }) => {
  const [registerDays, setRegisterDays] = useState<number | null>(null);

  useEffect(() => {
    const fetchRegisterDays = async () => {
      try {
        const userInfo = await fetchUserInfo(userId);
        const regDate = userInfo?.regDate;

        if (regDate) {
          const today = new Date();
          const regDateObj = new Date(
            `${regDate.substring(0, 4)}-${regDate.substring(4, 6)}-${regDate.substring(6, 8)}`
          );

          const diffTime = today.getTime() - regDateObj.getTime();
          const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24));
          setRegisterDays(diffDays);
        } else {
          console.warn('Registration date is not available.');
        }
      } catch (error) {
        console.error('Error fetching register days:', error);
      }
    };

    fetchRegisterDays();
  }, [userId]);

  return (
    <>
      {registerDays === 0
        ? '我认为今天很幸运，因为遇见了你.'
        : `我们已经相遇了 ${registerDays ?? '加载中...'} 天.🎇`}
    </>
  );
};
