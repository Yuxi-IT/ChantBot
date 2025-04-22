import { postEvent, on } from '@telegram-apps/sdk';
import React, { useEffect, useState } from 'react';
import { initData } from '@telegram-apps/sdk-react';
import { Button, List, Card } from '@telegram-apps/telegram-ui';
import { Loading } from '@/components/Loading.tsx';
import { fetchUserInfo } from '@/api/UserInfo';

postEvent('web_app_setup_back_button', { is_visible: true });

on('back_button_pressed', () => {
    window.history.back();
});

interface AmountChange {
    changeTime: string;
    balanceAfterChange: string;
    changeAmount: string;
    changeInfo: string;
}

const CustomListItem: React.FC<{ change: AmountChange; index: number; onCopy: (text: string) => void }> = ({ change, index, onCopy }) => {
    const [loading, setLoading] = useState(true);

    useEffect(() => {
      const timer = setTimeout(() => {
        setLoading(false);
      }, 800);
  
      return () => clearTimeout(timer);
    }, []);
  
    if (loading) {
      return <Loading />;
    }
    return (
        <Card style={styles.infocard}>
            <div style={styles.row}>
                <div style={styles.index}>{index + 1}</div>
                <div style={styles.textContainer}>
                    <p><strong>变更时间:</strong> {change.changeTime}</p>
                    <p><strong>剩余余额:</strong> {change.balanceAfterChange}</p>
                    <p><strong>更改金额:</strong> {change.changeAmount}</p>
                    <p><strong>更改事件:</strong> {change.changeInfo}</p>
                </div>
                <Button
                    style={styles.copyButton}
                    onClick={() =>
                        onCopy(
                            `${change.changeTime} | ${change.balanceAfterChange} | ${change.changeAmount} | ${change.changeInfo}`
                        )
                    }
                >
                    Copy
                </Button>
            </div>
        </Card>
    );
};

const UserBillPage: React.FC = () => {
    const [amountChanges, setAmountChanges] = useState<AmountChange[]>([]);
    const userId = initData.state()?.user?.id;

    useEffect(() => {
        if (!userId) return;

        const fetchData = async () => {
            const data = await fetchUserInfo(userId.toString());
            setAmountChanges(data?.amountChange || []);            
        };

        fetchData();
    }, [userId]);

    const copyToClipboard = (text: string) => {
        navigator.clipboard.writeText(text).then(() => {
            alert('Copied to clipboard');
        }).catch(err => {
            console.error('Could not copy text: ', err);
        });
    };

    return (
        <div>
            <div style={{ textAlign: 'center' }}>
                <h2>我的钱从哪来？又去哪了？</h2>
                <span style={{ fontSize: 130 }}>🧐</span>
            </div>
            {amountChanges.length === 0 ? (
                <Loading />
            ) : (
                <List>
                    {amountChanges.map((change, index) => (
                        <CustomListItem key={index} index={index} change={change} onCopy={copyToClipboard} />
                    ))}
                </List>
            )}
        </div>
    );
};

// 自定义样式
import { CSSProperties } from 'react';

const styles: { [key: string]: CSSProperties } = {
    infocard: {
        padding: '10px',
        borderRadius: '10px',
        boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
        backgroundColor: 'rgba(0,0,0,0.05)',
        width: '90%',
        margin: '10px 0 10px 10px' 
    },
    row: {
        display: 'flex',
        alignItems: 'center',
    },
    index: {
        minWidth: '30px',
        textAlign: 'center',
        fontWeight: 'bold',
        marginRight: '10px',
    },
    textContainer: {
        flex: 1,
    },
    copyButton: {
        minWidth: '80px',
    },
};

export default UserBillPage;
