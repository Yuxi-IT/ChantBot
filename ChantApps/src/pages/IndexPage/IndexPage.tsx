import { Section, Cell, Image, List } from '@telegram-apps/telegram-ui';
import type { FC } from 'react';
import { useState, useEffect } from 'react';

import { Link } from '@/components/Link/Link.tsx';
import { Page } from '@/components/Page.tsx';
import { initData } from '@telegram-apps/sdk-react';
import { Loading } from '@/components/Loading.tsx';
import { CheckInStatus } from '@/components/CheckInComponents';
import { RegisterDays } from '@/components/RegisterDaysProps';
import { UserInfoDisplay } from '@/components/UserDetails';

export const IndexPage: FC = () => {
  const [loading, setLoading] = useState(() => {
    const hasVisited = localStorage.getItem('hasVisited');
    return !hasVisited;
  });

  useEffect(() => {
    if (loading) {
      const timer = setTimeout(() => {
        setLoading(false);
        localStorage.setItem('hasVisited', 'true');
      }, 800);

      return () => clearTimeout(timer);
    }
  }, [loading]);

  if (loading) {
    return <Loading />;
  }

  var userData = initData.state()?.user;

  return (
    <Page back={false}>
      <List>
        <Section
          header="欢迎回到 浅吟 App."
          footer="你可以使用这个App使用与 @Chant_Bot 完全一致的服务，但将有更好的体验."
        >
          <Link to="/user-data">
            <Cell
              before={<Image src={userData?.photoUrl} style={{ backgroundColor: '#007AFF', borderRadius: 30 }} />}
              subtitle={<RegisterDays userId={userData?.id?.toString() ?? ''} />}
            >
              {userData?.firstName}
              {userData?.lastName}
            </Cell>
          </Link>
        </Section>
        <Section>
          {userData?.id && <CheckInStatus userId={userData.id.toString()} />}
          <Link to="/user-bill">
            <Cell>🧾 看看钱都哪去了</Cell>
          </Link>
        </Section>
        <Section header="User Details">
          <UserInfoDisplay userId={userData?.id?.toString() ?? ''} />
        </Section>
        <Section header="Links">
          <Link to="https://t.me/Chant_Bot">
            <Cell>@Chant_Bot</Cell>
          </Link>
          <Link to="https://t.me/SmaZone">
            <Cell>@SmaZone(Admin)</Cell>
          </Link>
          <Link to="https://t.me/NovaSGK">
            <Cell>@NovaSGK(Gourp)</Cell>
          </Link>
          <Link to="https://t.me/hiNova888">
            <Cell>@HiNova888(Channel)</Cell>
          </Link>
        </Section>
      </List>
    </Page>
  );
};
