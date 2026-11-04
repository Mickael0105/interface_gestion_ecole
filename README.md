# App Micka - Gestion Scolaire

Application WPF (Windows Presentation Foundation) de gestion scolaire développée en C# avec .NET 8. Il s'agit d'une interface de gestion d'école avec authentification, suivi des étudiants, gestion des enseignants, des classes, des matières et des notes.

Le projet est encore à l'état d'interface fonctionnelle/prototype : il n'utilise pas de base de données réelle pour le moment et repose sur une persistance locale en mémoire/JSON.

## Aperçu

Le logiciel permet à un utilisateur de :
- se connecter avec un rôle (Admin, Enseignant, Étudiant)
- gérer les étudiants, enseignants et classes
- ajouter des matières et affecter des matières à une classe
- saisir et consulter des notes
- générer un bulletin HTML par étudiant
- exporter des données au format CSV
- basculer entre thème clair et sombre

## Fonctionnalités principales

### Authentification
- Connexion basée sur des comptes prédéfinis
- Gestion des rôles : Admin, Teacher, Student
- Blocage automatique après plusieurs tentatives

Comptes inclus :
- Admin : admin / admin123
- Enseignant : enseignant / enseignant123
- Étudiant : etudiant / etudiant123

### Gestion scolaire
- Ajout, modification et suppression d'étudiants
- Ajout, modification et suppression d'enseignants
- Gestion des classes et matières
- Attribution des matières à une classe
- Saisie de notes sur 20
- Calcul des moyennes
- Génération de bulletin HTML

### Interface utilisateur
- Conception WPF moderne avec XAML
- Palette de couleurs stylisée et thème dynamique
- Tableau de bord avec statistiques
- Navigation par onglets
- Affichage de données en tableaux et listes

## Stack technique

- C#
- .NET 8
- WPF / XAML
- JSON pour la persistance locale
- Visual Studio 2022

## Structure du projet

```text
interface_gestion_ecole/
├── App.xaml
├── App.xaml.cs
├── HomeWindow.xaml
├── HomeWindow.xaml.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── WelcomeWindow.xaml
├── WelcomeWindow.xaml.cs
├── interface.csproj
├── Models/
│   └── Entities.cs
├── Services/
│   ├── AuthenticationService.cs
│   ├── InMemoryStore.cs
│   └── JsonDataService.cs
├── Converters/
│   └── UiConverters.cs
├── Themes/
├── .gitignore
├── .gitattributes
└── Ressources images / fichiers visuels
```

## Modèle de données

Le projet contient les classes principales suivantes dans `Models/Entities.cs` :
- `Student`
- `Teacher`
- `SchoolClass`
- `Subject`
- `Grade`

La persistance est assurée par `JsonDataService` et `InMemoryStore`, qui sauvegardent les données dans le dossier ApplicationData de l'utilisateur (sous `MyAppDashboard/data.json`).

## Rôles et permissions

Le système applique des permissions selon le rôle connecté :
- Admin : accès complet
- Teacher : accès restreint à certaines fonctionnalités
- Student : accès limité au tableau de bord et au suivi personnel

## Prérequis

Pour exécuter le projet, il faut :
- Windows
- Visual Studio 2022
- Charge de travail : .NET desktop development
- .NET 8 SDK

## Exécution

1. Ouvrir le projet dans Visual Studio.
2. Vérifier que le framework cible est bien `net8.0-windows`.
3. Restaurer les packages NuGet si nécessaire.
4. Lancer le projet avec F5.

## Remarques

- Ce projet est une interface de démonstration/prototype.
- Il n'y a pas encore de base de données relationnelle.
- Les données sont stockées localement en JSON, ce qui convient pour une version de démonstration ou une maquette de gestion scolaire.
- Le projet est orienté Windows, car il utilise WPF.

## Objectif du projet

Le but principal est de fournir une interface utilisateur complète pour gérer les opérations de base d'une école : élèves, enseignants, classes, matières, notes et impressions de bulletins.

## État du projet

Prototype fonctionnel orienté démonstration / interface utilisateur.

## Auteur

Projet développé par Mickael0105.

---

Si vous souhaitez, je peux aussi vous préparer :
- une version du README en anglais,
- un README plus professionnel avec badges et captures d'écran,
- ou une version plus concise pour GitHub.
