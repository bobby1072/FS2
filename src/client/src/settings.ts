import { UserManagerSettings } from 'oidc-client-ts';

const { protocol, host } = window.location;

interface ISettings {
  oidcSettings: Omit<UserManagerSettings, 'authority' | 'client_id'>;
}

const Settings: ISettings = {
  oidcSettings: {
    response_type: 'code',
    scope: 'openid profile email',
    loadUserInfo: true,
    automaticSilentRenew: true,
    redirect_uri: `${protocol}//${host}/oidc-signin`,
    silent_redirect_uri: `${protocol}//${host}/oidc-silent-renew`,
  },
};

export default Settings;
