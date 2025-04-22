import axios from 'axios';

var BaseApiUrl = "https://localhost:7132/api";

interface AmountChange {
  changeTime: string;
  balanceAfterChange: string;
  changeAmount: string;
  changeInfo: string;
}

export interface UserInfo {
  id: string;
  amount: number;
  level: number;
  vipLevel: number;
  invites: string[];
  paymentCode: string;
  lastCheckIn: string;
  regDate: string;
  amountChange: AmountChange[];
  inviteCode: string;
}

export const fetchUserInfo = async (userId: string): Promise<UserInfo | null> => {
  try {
    const response = await axios.get(`${BaseApiUrl}/info?id=${userId}&tick=${Math.floor(Date.now() / 1000)}`);
    if (response.data.code === 200) {
      const rawData = response.data.data;

      const parsedAmountChange = (rawData.amountChange || []).map((item: string) => {
        if (!item.trim()) return null;
        const [changeTime, balanceAfterChange, changeAmount, changeInfo] = item.split('|');
        return { changeTime, balanceAfterChange, changeAmount, changeInfo };
      }).filter(Boolean) as AmountChange[];

      return {
        id: rawData.id,
        amount: rawData.amount,
        level: rawData.level,
        vipLevel: rawData.vipLevel,
        invites: rawData.invites,
        paymentCode: rawData.paymentCode,
        lastCheckIn: rawData.lastCheckIn,
        regDate: rawData.regDate,
        amountChange: parsedAmountChange,
        inviteCode: rawData.inviteCode,
      };
    } else {
      throw new Error('Failed to fetch user info, status code not 200');
    }
  } catch (error) {
    console.error('Error fetching user info:', error);
    return null;
  }
};

export const CheckIn = async (userId: string): Promise<number> => {
    try {
        const response = await axios.get(`${BaseApiUrl}/checkin?id=${userId}&tick=${Math.floor(Date.now() / 1000)}`);
        const result = response.data.data;

        return result;

    } catch (error) {
        console.error('Error during check-in:', error);
        return -1;
    }
};

export const getLevelName = (level: number): string => {
    switch (level) {
        case 1:
            return "⭐赌徒⭐";
        case 2:
            return "🌟赌棍🌟";
        case 3:
            return "✨赌侠✨";
        case 4:
            return "👹赌怪👹";
        case 5:
            return "👑赌王👑";
        case 6:
            return "🗡赌圣🗡";
        case 7:
            return "💎赌神💎";
        case 8:
            return "🌩️赌仙🌩️";
        case -1:
            return "🌩️场务管理员🌩️";
        default:
            return "未知";
    }
};

export const getVipName = (vip: number): string => {
    switch (vip) {
        case 1:
            return "💲铂金会员💲";
        case 2:
            return "🏅黄金会员🏅";
        case 3:
            return "✨闪星会员✨";
        case 4:
            return "💎钻石会员💎";
        case 5:
            return "👑黑金会员👑";
        default:
            return "👤用户👤";
    }
};