# Manuel utilisateur — Property Manager

> Guide pratique de l’application **Property Manager** : immeubles, unités, locataires, baux, loyers, paiements, dépenses, documents, messages, agenda et administration.
>
> Destiné aux utilisateurs finaux et aux administrateurs. Aucune connaissance en programmation n’est nécessaire.
>
> 💡 Ce manuel est disponible **dans l’application** : cliquez sur le bouton `?` dans la **barre supérieure** pour l’ouvrir dans une boîte de dialogue, sans quitter la page en cours.

---

## Sommaire

- [1. Premiers pas](#primeiros-passos)
- [2. Vue d’ensemble et navigation](#navegacao)
- [3. Tableau de bord](#dashboard)
- [4. Immeubles](#imoveis)
- [5. Unités](#fracoes)
- [6. Locataires](#inquilinos)
- [7. Baux](#arrendamentos)
- [8. Loyers et paiements](#rendas-pagamentos)
- [9. Dépenses](#despesas)
- [10. Contacts, propriétaire et documents](#contactos-documentos)
- [11. Messages, notifications et agenda](#mensagens)
- [12. Modèles de lettres](#modelos-cartas)
- [13. Calculateurs et cartes](#calculadoras)
- [14. Administration](#administracao)
- [15. Dépannage](#resolucao)
- [16. Questions fréquentes](#faq)
- [Annexe A — Routes principales](#rotas)

---

## 1. Premiers pas {#primeiros-passos}

### 1.1 Se connecter

1. Ouvrez l’adresse de l’application dans votre navigateur.
2. Saisissez votre **adresse e-mail** et votre **mot de passe**.
3. Cliquez sur **Se connecter**.
4. Si vous n’avez pas encore de compte, utilisez le lien d’**inscription** sur l’écran d’authentification (dans certaines installations, le compte est créé par l’administrateur).

### 1.2 Choisir la langue

1. Dans la barre supérieure, utilisez le **sélecteur de langue**.
2. Choisissez **Portugais**, **Anglais**, **Espagnol** ou **Français**.
3. Le choix est enregistré dans les paramètres de l’application et recharge l’interface.

> 💡 Le contenu que vous saisissez (noms, adresses, descriptions) **n’est pas traduit** : seule l’interface l’est. Vous pouvez avoir l’interface en français et vos données en portugais.

### 1.3 Créer votre premier immeuble

1. Dans le menu latéral, ouvrez **Immeubles** (`/imoveis`).
2. Cliquez sur **Nouveau** (le bouton vert avec `+`).
3. Renseignez les informations d’identification et l’**adresse** de l’immeuble.
4. Cliquez sur **Enregistrer**.

### 1.4 Créer votre première unité

1. Ouvrez **Immeubles** et sélectionnez l’immeuble souhaité.
2. Ajoutez une **unité**, par exemple un appartement ou une boutique.
3. Renseignez l’identification de l’unité, le **loyer**, le **type** et la **situation**.
4. Vous pouvez téléverser des **images** de l’unité dans le gestionnaire d’images (`/fracoes/imagesmanager`).

### 1.5 Créer votre premier locataire

1. Dans le menu latéral, ouvrez **Locataires** (`/inquilinos`).
2. Cliquez sur **Nouveau** et renseignez les données personnelles et les coordonnées.
3. Le cas échéant, associez un **garant**.
4. Enregistrez.

### 1.6 Créer votre premier bail

1. Ouvrez **Baux** (`/arrendamentos`).
2. Cliquez sur **Nouveau**.
3. Associez l’**unité** et le **locataire**, puis indiquez la date de début, la durée et le montant du loyer.
4. Enregistrez. À partir de là, vous pouvez générer le contrat et les lettres associées.

### 1.7 Tableau « je veux faire X → je vais à Y »

| Je veux… | Je vais à… | Route |
|---|---|---|
| Voir le résumé général | Accueil | `/` |
| Gérer les immeubles | Immeubles | `/imoveis` |
| Gérer les unités et les images | Immeubles → unité | `/fracoes/viewunits` |
| Gérer les locataires | Locataires | `/inquilinos` |
| Gérer les baux | Baux | `/arrendamentos` |
| Traiter les loyers mensuels | Paiements → Traiter les loyers | `/monthlyrentprocessing` |
| Saisir un paiement | Paiements → Maintenance | `/recebimentos/manutencao` |
| Gérer les dépenses | Dépenses → Maintenance | `/despesas/manutencao` |
| Modifier les lettres | Administration → Modèles de lettres | `/LetterTemplates` |
| Voir les notifications | Cloche dans la barre supérieure | `/notifications` |
| Ouvrir l’agenda | Agenda | `/scheduler` |
| Paramètres et sauvegardes | Maintenance | `/admin/appsettings` |

---

## 2. Vue d’ensemble et navigation {#navegacao}

### 2.1 Barre supérieure

Elle contient :

- Le bouton d’**ouverture/fermeture du menu latéral**.
- Le nom de l’application.
- Le bouton d’**aide** (`?`) — ouvre ce manuel sans quitter la page.
- La **cloche de notifications**.
- Le **sélecteur de langue**.
- Le **nom d’utilisateur** (lien vers la gestion du compte).
- Le bouton de **thème clair/sombre**.
- L’option de **déconnexion**.

### 2.2 Menu latéral

Le menu est organisé en groupes :

- **Général** — Accueil, Immeubles, Locataires, Baux, Dépenses, Paiements, Calculateurs, Contacts, Documents, Messages et Agenda.
- **Administration / maintenance** — Journaux, Paramètres, Sauvegarde de la base de données et Sauvegarde des fichiers.

Les groupes avec des sous-éléments se déplient/replient au clic. L’élément actif est mis en évidence.

### 2.3 Grilles

Presque toutes les listes utilisent une grille avec une barre d’outils. Les opérations les plus courantes sont :

| Action | Comment |
|---|---|
| Rechercher | Zone de recherche dans la barre d’outils |
| Trier | Cliquer sur l’en-tête d’une colonne |
| Exporter | `PdfExport` ou `ExcelExport` dans la barre d’outils |
| Déplier/replier les lignes | Boutons de dépliage/repliage dans la barre |
| Créer | Bouton **Nouveau** |
| Modifier / supprimer | Boutons sur la ligne ou menu contextuel |

> 💡 Dans certaines grilles, un clic droit ouvre un **menu contextuel** avec des actions rapides (lettres de retard, mise à jour des loyers, etc.).

---

## 3. Tableau de bord {#dashboard}

La page d’accueil (`/`) présente les indicateurs généraux : résumé des immeubles, locataires, baux, loyers et dépenses, ainsi que les alertes et l’activité récente.

- Utilisez les cartes pour accéder directement à la zone correspondante.
- Si un chiffre n’est pas à jour, rechargez la page (F5).
- Les **alertes** du tableau de bord sont les mêmes que celles de la cloche de notifications : ce ne sont pas des avertissements différents par page.

---

## 4. Immeubles {#imoveis}

Page : **Immeubles** (`/imoveis`). Une vue alternative est également disponible sur `/properties`.

### 4.1 Qu’est-ce qu’un immeuble

L’**immeuble** est le niveau supérieur : un bâtiment, une maison ou un ensemble. Les **unités** se trouvent à l’intérieur.

### 4.2 Créer et modifier

1. Cliquez sur **Nouveau**.
2. Renseignez l’identification et l’adresse.
3. Enregistrez.
4. Pour modifier, utilisez le bouton d’édition sur la ligne de la grille.

### 4.3 Exporter et rechercher

La grille permet de rechercher, trier et exporter la liste au format PDF.

---

## 5. Unités {#fracoes}

Pages : **Unités** (`/fracoes/viewunits`) et **Gestionnaire d’images** (`/fracoes/imagesmanager`).

### 5.1 Qu’est-ce qu’une unité

Une **unité** est une division à l’intérieur de l’immeuble, par exemple un appartement, une boutique ou un garage. Le **bail** et le **loyer** sont associés à l’unité.

### 5.2 Champs principaux

- Identification et description.
- **Type d’unité** et **situation** (par exemple disponible ou louée).
- **Loyer** et valeur **IMI** (impôt foncier municipal).
- **Assurance** associée.
- **Active / inactive**.

### 5.3 Images

1. Ouvrez le gestionnaire d’images (`/fracoes/imagesmanager`).
2. Sélectionnez l’unité.
3. Téléversez une ou plusieurs images.
4. Confirmez le téléversement ; les images sont disponibles dans la vue de l’unité.

> 💡 Si le téléversement échoue avec `Access to the path ... is denied`, le dossier de téléversement du serveur n’est pas accessible en écriture : demandez à l’administrateur de corriger les autorisations IIS.

---

## 6. Locataires {#inquilinos}

Page : **Locataires** (`/inquilinos`).

### 6.1 Créer un locataire

1. Cliquez sur **Nouveau**.
2. Renseignez le **nom**, les **coordonnées** et les données d’identification.
3. Le cas échéant, associez un **garant**.
4. Enregistrez.

### 6.2 La fiche du locataire

La fiche regroupe :

- Les données personnelles et les coordonnées.
- Les **documents** téléversés (`DocumentosInquilino`).
- Le **solde courant** (`SaldoCorrenteInquilino`) : ce qui est dû ou ce qui est en crédit.
- Les baux associés.

### 6.3 Mettre à jour le montant du loyer

Deux possibilités :

- **Manuellement** : dans le menu contextuel du locataire, choisissez **Mettre à jour le montant du loyer** (`/updaterentvalue/{id}`).
- **Par lettre** : via **Baux → Mise à jour des loyers** (`/updaterentsletter`).

### 6.4 Lettres rapides

Le menu contextuel du locataire permet de générer directement :

- **Lettre de retard de paiement**.
- **Lettre de révocation / opposition au contrat**.
- **Lettre de mise à jour des loyers**.

> ⚠️ Avant de générer une lettre, vérifiez que les données du bail et du locataire sont correctes : la lettre utilise les valeurs enregistrées.

---

## 7. Baux {#arrendamentos}

Page : **Baux** (`/arrendamentos`).

### 7.1 Qu’est-ce qu’un bail

Un bail relie un **locataire** à une **unité**, avec une date de début, une durée et un montant de loyer. L’application avertit lorsqu’un contrat arrive à échéance ou lorsque des loyers sont en retard.

### 7.2 Créer un contrat

1. Cliquez sur **Nouveau**.
2. Sélectionnez l’**unité** et le **locataire**.
3. Saisissez la **date de début**, la **durée** et le **montant du loyer**.
4. Enregistrez.

### 7.3 Actions dans la grille

La barre d’outils et le menu contextuel permettent de :

- **Voir le contrat** (`Voir le contrat`).
- **Prolonger la durée du contrat**.
- **Mettre à jour les loyers** et générer la lettre correspondante.
- Consulter le **solde courant du locataire** (`/saldocorrenteinquilino`).

### 7.4 Coefficients de mise à jour

Dans **Baux → Coefficients** (`/updatecoefficients`), vous pouvez gérer les coefficients utilisés pour calculer les mises à jour annuelles du loyer.

> 💡 Selon la configuration de l’installation, le menu *Baux* peut apparaître avec des sous-menus (Maintenance et Mise à jour des loyers) au lieu d’un seul élément.

---

## 8. Loyers et paiements {#rendas-pagamentos}

Pages : **Paiements → Maintenance** (`/recebimentos/manutencao`), **Traiter les loyers** (`/monthlyrentprocessing`) et **Tableau de bord des paiements** (`/payments/dashboard`).

### 8.1 Saisir un paiement

1. Ouvrez **Paiements → Maintenance**.
2. Cliquez sur **Nouveau** (ou équivalent).
3. Sélectionnez le **locataire**, le **concept**, le **montant** et la **date**.
4. Enregistrez.

La grille permet de rechercher, trier et exporter au format Excel.

### 8.2 Traitement mensuel des loyers

1. Ouvrez **Paiements → Traiter les loyers** (`/monthlyrentprocessing`).
2. Confirmez le **mois/année** et les critères.
3. Vérifiez l’aperçu des loyers à traiter.
4. Confirmez le traitement.

> ⚠️ Le traitement par lot crée plusieurs enregistrements à la fois. Vérifiez toujours la liste avant de confirmer.

### 8.3 Tableau de bord des paiements

Le tableau de bord (`/payments/dashboard`) résume les paiements par période et permet de suivre ce qui a été payé et ce qui reste dû.

---

## 9. Dépenses {#despesas}

Pages : **Dépenses → Maintenance** (`/despesas/manutencao`), **Types de dépense** (`/despesas/tipodespesas`) et **Tableau de bord des dépenses** (`/expenses/dashboard`).

### 9.1 Saisir une dépense

1. Ouvrez **Dépenses → Maintenance**.
2. Cliquez sur **Nouveau**.
3. Renseignez la **description**, le **montant**, la **date** et le **type de dépense**.
4. Enregistrez.

### 9.2 Types de dépense

Dans **Types de dépense**, vous pouvez créer, modifier et organiser les catégories utilisées lors de la saisie.

### 9.3 Graphiques

Le tableau de bord affiche les dépenses par type et au fil du temps (graphiques en courbes et en secteurs). Survolez un point pour voir la valeur exacte.

---

## 10. Contacts, propriétaire et documents {#contactos-documentos}

### 10.1 Contacts

Page : `/contactos`. Enregistre les contacts utiles (administrateurs, copropriétés, fournisseurs, etc.).

### 10.2 Propriétaire

Pages : `/Proprietario` et `/ProprietarioPage`. Regroupent les données du propriétaire de l’immeuble.

### 10.3 Documents

Page : **Documents** (`/documentos`). Permet d’enregistrer et de téléverser les documents généraux de l’application. Pour les documents d’un locataire, utilisez sa fiche.

---

## 11. Messages, notifications et agenda {#mensagens}

### 11.1 Notifications

La **cloche** dans la barre supérieure ouvre le centre de notifications. Vous pouvez aussi ouvrir la page complète sur `/notifications`.

- Les notifications comprennent les alertes opérationnelles (rendez-vous, loyers, lettres, contrats).
- Chaque alerte peut inclure une **action suggérée** — par exemple ouvrir la fiche du locataire, enregistrer un paiement ou générer la lettre correspondante.
- Les alertes comprennent : loyers en retard, lettre de rappel manquante, loyer à échoir prochainement, lettre de révocation sans réponse et bail marqué comme émis sans document généré.
- Les messages non lus sont mis en évidence.
- L’ouverture d’une notification la marque comme lue.

### 11.2 Messages

Page : **Messages** (`/messages-modern` ; l’ancienne version se trouve sur `/messages`). Permet d’envoyer et de consulter des messages.

### 11.3 Agenda

Page : **Agenda** (`/scheduler`). Permet d’enregistrer des rendez-vous et de les consulter dans un calendrier, avec recherche d’événements (`/scheduler/search-events`).

---

## 12. Modèles de lettres {#modelos-cartas}

Page : **Modèles de lettres** (`/LetterTemplates`).

### 12.1 Que sont les modèles

Ce sont des fichiers `.dotx` utilisés pour générer automatiquement des lettres et des contrats (avis de retard, mise à jour des loyers, révocations, baux, etc.).

### 12.2 Modifier un modèle

1. Ouvrez **Modèles de lettres**.
2. Sélectionnez le modèle dans la liste.
3. Modifiez le document dans l’éditeur de texte.
4. Pour voir le résultat avec des valeurs d’exemple, utilisez **Aperçu**.
5. **Enregistrez** uniquement lorsque vous êtes satisfait.

### 12.3 Champs de fusion

Les modèles contiennent des **champs de fusion** (par exemple `«NomeInquilino»`) qui sont remplis avec les données du bail ou du locataire lors de la génération de la lettre. Ne supprimez pas ces champs sans nécessité.

> ⚠️ La suppression d’un modèle est définitive. Confirmez toujours avant de supprimer.

---

## 13. Calculateurs et cartes {#calculadoras}

### 13.1 Calculateurs

Le menu **Calculateurs** comprend :

- **IMI** (`/CalculadoraIMI`) — calcul de l’impôt municipal sur les immeubles.
- **VPT** (`/CalculadoraVPT`) — calcul de la valeur patrimoniale fiscale.
- **Gestion des coefficients IMI** (`/ManutencaoCoeficientesIMI`).

### 13.2 Cartes

Les pages `/maps` et `/gmap` affichent la localisation des immeubles sur une carte.

---

## 14. Administration {#administracao}

### 14.1 Paramètres

Page : **Paramètres** (`/admin/appsettings`). Permet de configurer les paramètres généraux, comme la langue par défaut et l’ajustement automatique des loyers.

### 14.2 Sauvegardes

- **Base de données** (`/BackupDatabase`) — crée une copie de la base de données.
- **Fichiers / téléversements** (`/backupuploads/1`) — crée des copies des dossiers de fichiers téléversés.

> 💡 Effectuez des sauvegardes régulièrement et conservez les copies hors du serveur.

### 14.3 Journaux

- **Journaux de l’application** (`/admin/viewapplogs`) — erreurs, avertissements et événements.
- **Informations de connexion** (`/admin/logininfo`) — enregistrements des connexions.

### 14.4 Tableaux et tableaux de bord

- **Panneau d’administration** (`/admin/adminpanel`) et **configuration des tables** (`/admin/configapptables`).
- **Tableau de bord récapitulatif** (`/summarydashboard`).

---

## 15. Dépannage {#resolucao}

| Symptôme | Cause probable | Que faire |
|---|---|---|
| « Accès refusé au chemin… » lors du téléversement | Le dossier de téléversement du serveur n’est pas accessible en écriture | Demandez à l’administrateur de corriger les autorisations IIS |
| La grille ne charge pas | API indisponible ou connexion à la base de données impossible | Consultez les journaux (`/admin/viewapplogs`) et réessayez |
| Le manuel ne s’ouvre pas | Fichier `docs/Help*.md` absent sur le serveur | Vérifiez que le dossier `docs` a été publié avec l’application |
| Des données ont « disparu » | Filtre actif dans la grille ou utilisateur différent | Effacez les filtres et confirmez le compte utilisé pour la connexion |
| Les boutons ne répondent pas | Page chargée avant que la connexion Blazor soit prête | Attendez une seconde et réessayez ; rechargez si le problème persiste |

---

## 16. Questions fréquentes {#faq}

**Dois-je savoir programmer pour utiliser l’application ?**
Non. Tout se fait à l’aide de formulaires et de grilles.

**Puis-je utiliser l’application dans une autre langue ?**
Oui. Portugais, anglais, espagnol et français. Utilisez le sélecteur de langue dans la barre supérieure.

**Mes données sont-elles supprimées lorsque je change de langue ?**
Non. Seule l’interface change.

**Que se passe-t-il si je supprime un locataire avec des contrats ?**
L’application avertit ou empêche la suppression, selon les règles configurées. Résolvez d’abord les contrats associés.

**Comment savoir si un loyer a été payé ?**
Consultez le **solde courant du locataire** et le **tableau de bord des paiements**.

**Le manuel est-il mis à jour sans redémarrer l’application ?**
Oui. Le fichier est relu du disque à chaque ouverture de la boîte de dialogue.

---

## Annexe A — Routes principales {#rotas}

| Zone | Route |
|---|---|
| Accueil | `/` |
| Immeubles | `/imoveis`, `/properties` |
| Unités | `/fracoes/viewunits`, `/fracoes/imagesmanager` |
| Locataires | `/inquilinos`, `/Inquilinos/Detalhe` |
| Baux | `/arrendamentos`, `/updaterentsletter`, `/updatecoefficients` |
| Loyers | `/monthlyrentprocessing`, `/saldocorrenteinquilino` |
| Paiements | `/recebimentos/manutencao`, `/payments/dashboard` |
| Dépenses | `/despesas/manutencao`, `/despesas/tipodespesas`, `/expenses/dashboard` |
| Contacts | `/contactos` |
| Documents | `/documentos` |
| Messages | `/messages-modern`, `/messages` |
| Notifications | `/notifications` |
| Agenda | `/scheduler` |
| Modèles de lettres | `/LetterTemplates` |
| Calculateurs | `/CalculadoraIMI`, `/CalculadoraVPT`, `/ManutencaoCoeficientesIMI` |
| Cartes | `/maps`, `/gmap` |
| Paramètres | `/admin/appsettings` |
| Sauvegardes | `/BackupDatabase`, `/backupuploads/1` |
| Journaux | `/admin/viewapplogs`, `/admin/logininfo` |
