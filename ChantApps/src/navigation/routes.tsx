import type { ComponentType, JSX } from 'react';

import { IndexPage } from '@/pages/IndexPage/IndexPage';
import { InitDataPage } from '@/pages/UserDataPage';

import UserBillPage from '@/pages/UserBillPage';

interface Route {
  path: string;
  Component: ComponentType;
  title?: string;
  icon?: JSX.Element;
}

export const routes: Route[] = [
  { path: '/', Component: IndexPage },
  { path: '/user-data', Component: InitDataPage, title: '个人信息' },
  { path: '/user-bill', Component: UserBillPage, title: '余额流水' }
];
