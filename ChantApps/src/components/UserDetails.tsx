import { FC, Key, useEffect, useState } from 'react';
import { Section, Cell, List } from '@telegram-apps/telegram-ui';
import { Loading } from '@/components/Loading.tsx';
import { fetchUserInfo, UserInfo, getLevelName, getVipName} from '@/api/UserInfo';

interface UserInfoDisplayProps {
  userId: string;
}

export const UserInfoDisplay: FC<UserInfoDisplayProps> = ({ userId }) => {
  const [loading, setLoading] = useState(true);
  const [userInfo, setUserInfo] = useState<UserInfo | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      const data = await fetchUserInfo(userId);
      setUserInfo(data);
      setLoading(false);
    };

    fetchData();
  }, [userId]);

  if (loading) {
    return <Loading />;
  }

  if (!userInfo) {
    return (
      <Section>
        <Cell subtitle="无法加载用户信息，请稍后重试。">加载失败</Cell>
      </Section>
    );
  }

  return (
    <List>
      <Section>
        <Cell subtitle={getLevelName(userInfo.level)}>用户等级</Cell>
        <Cell subtitle={getVipName(userInfo.vipLevel)}>VIP 等级</Cell>
        <Cell subtitle={`${userInfo.amount}⭐`}>账户余额</Cell>
      </Section>
      <Section header="收款信息">
        <Cell 
        subtitle={userInfo.paymentCode}
        onClick={() => {
        navigator.clipboard.writeText(userInfo.paymentCode);
        alert('收款信息已复制到剪贴板');
        }}
      />
      </Section>
    <Section header="邀请码">
      <Cell
        subtitle={userInfo.inviteCode}
        onClick={() => {
        navigator.clipboard.writeText(userInfo.inviteCode);
        alert('邀请码已复制到剪贴板');
        }}
      />
    </Section>
      <Section header="邀请用户">
        {userInfo.invites.length ? (
          userInfo.invites.map((invite: string, index: Key) => (
            <Cell key={index} subtitle={invite} />
          ))
        ) : (
          <Cell subtitle="暂无邀请记录" />
        )}
      </Section>
    </List>
  );
};
